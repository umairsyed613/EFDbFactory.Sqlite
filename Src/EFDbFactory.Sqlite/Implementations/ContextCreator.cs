using System;
using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFDbFactory.Sqlite
{
    public class ContextCreator<T> where T : CommonDbContext
    {
        private readonly SqliteConnection _outerConnection;
        private readonly SqliteTransaction _transaction;
        private readonly ILoggerFactory _loggerFactory;
        private readonly bool _enableSensitiveDataLogging;

        public ContextCreator(SqliteConnection connection)
        {
            _outerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = null;
        }

        public ContextCreator(SqliteConnection connection, SqliteTransaction transaction)
        {
            _outerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = transaction;
        }

        public ContextCreator(SqliteConnection connection, SqliteTransaction transaction, ILoggerFactory loggerFactory,
            bool enableSensitiveDataLogging)
        {
            _outerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = transaction;
            _loggerFactory = loggerFactory;
            _enableSensitiveDataLogging = enableSensitiveDataLogging;
        }

        public ContextCreator(SqliteConnection connection, ILoggerFactory loggerFactory, bool enableSensitiveDataLogging)
        {
            _outerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = null;
            _loggerFactory = loggerFactory;
            _enableSensitiveDataLogging = enableSensitiveDataLogging;
        }

        public T GetReadOnlyWithNoTracking(bool inMemory = false)
        {
            var context = CreateDbContext(inMemory);
            context.ReadOnlyMode = true;
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.LazyLoadingEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        public T GetReadWriteWithDbTransaction(bool inMemory = false)
        {
            var context = CreateDbContext(inMemory);
            
            if (_transaction != null && !inMemory)
            {
                context.Database.UseTransaction(_transaction);
            }

            context.ChangeTracker.AutoDetectChangesEnabled = true;

            return context;
        }

        private T CreateDbContext(bool inMemory)
        {
            var options = new DbContextOptionsBuilder<T>();

            if (_loggerFactory != null)
            {
                options.UseLoggerFactory(_loggerFactory);

                options.EnableSensitiveDataLogging(_enableSensitiveDataLogging);
            }

            if (inMemory)
            {
                options.UseInMemoryDatabase(_outerConnection.Database);
            }
            else { options.UseSqlite(_outerConnection); }

            return (T) Activator.CreateInstance(typeof(T), options.Options);
        }
    }
}
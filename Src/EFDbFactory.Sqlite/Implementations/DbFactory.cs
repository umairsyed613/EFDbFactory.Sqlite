using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace EFDbFactory.Sqlite
{
    public class DbFactory : IDbFactory
    {
        private readonly string _connectionString;
        private readonly ILoggerFactory _loggerFactory;
        private readonly bool _enableSensitiveDataLogging;
        private bool _readOnly;
        private readonly bool _inMemory;

        public SqliteConnection Connection { get; private set; }
        public SqliteTransaction Transaction { get; private set; }

        public DbFactory(string connectionString) => _connectionString = connectionString;

        public DbFactory(string connectionString, ILoggerFactory loggerFactory, bool enableSensitiveDataLogging)
        {
            _connectionString = connectionString;
            _loggerFactory = loggerFactory;
            _enableSensitiveDataLogging = enableSensitiveDataLogging;
        }

        public DbFactory(string connectionString, bool inMemory) : this(connectionString)
        {
            _inMemory = inMemory;
        }

        public DbFactory(string connectionString, ILoggerFactory loggerFactory, bool enableSensitiveDataLogging, bool inMemory) : this(connectionString, loggerFactory, enableSensitiveDataLogging)
        {
            _inMemory = inMemory;
        }

        /// <summary>
        /// create factory with desired transaction isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IDbFactory> CreateTransactional(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) =>
            await CreateReadWriteWithTransactionLevel(isolationLevel);

        /// <summary>
        /// create factory with no transaction
        /// </summary>
        /// <returns></returns>
        public async Task<IDbFactory> CreateReadOnly() => await CreateReadOnlyConnection();


        public T FactoryFor<T>()
        where T : CommonDbContext =>
            _loggerFactory != null ?
                (_readOnly ? new ContextCreator<T>(Connection, _loggerFactory, _enableSensitiveDataLogging).GetReadOnlyWithNoTracking(_inMemory) :
                     new ContextCreator<T>(Connection, Transaction, _loggerFactory, _enableSensitiveDataLogging).GetReadWriteWithDbTransaction(_inMemory)) :
                (_readOnly ? new ContextCreator<T>(Connection).GetReadOnlyWithNoTracking(_inMemory) :
                     new ContextCreator<T>(Connection, Transaction).GetReadWriteWithDbTransaction(_inMemory));

        /// <summary>
        /// Commit the transaction. throw exception when transaction is null. will not commit the transaction if the factory is created CreateNoCommit
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void CommitTransaction()
        {
            if (Transaction == null)
            {
                throw new InvalidOperationException("Cannot commit null transaction");
            }

            Transaction.Commit();
        }

        private async Task<IDbFactory> CreateReadWriteWithTransactionLevel(IsolationLevel isolationLevel)
        {
            Connection = new SqliteConnection(_connectionString);
            await Connection.OpenAsync();
            Transaction = Connection.BeginTransaction(isolationLevel);
            _readOnly = false;
            return this;
        }

        private async Task<IDbFactory> CreateReadOnlyConnection()
        {
            Connection = new SqliteConnection(_connectionString);
            await Connection.OpenAsync();
            Transaction = null;
            _readOnly = true;
            return this;
        }

        public void Dispose()
        {
            Transaction?.Rollback();
            Connection?.Close();

            Connection?.Dispose();
            Transaction?.Dispose();
        }
    }
}
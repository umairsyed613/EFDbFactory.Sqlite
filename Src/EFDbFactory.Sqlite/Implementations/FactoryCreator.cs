using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EFDbFactory.Sqlite
{
    public class FactoryCreator : IFactoryCreator
    {
        private readonly string _connectionString;

        public FactoryCreator(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// create factory with desired transaction isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IFactoryCreator> Create(IsolationLevel isolationLevel) => await CreateReadWrite(isolationLevel);

        /// <summary>
        /// create factory with no transaction
        /// </summary>
        /// <returns></returns>
        public async Task<IFactoryCreator> Create() => await CreateReadOnly();

        public IDbFactory<T> FactoryFor<T>() where T : CommonDbContext => new DbFactory<T>(Connection, Transaction);

        public void CommitTransaction()
        {
            if (Transaction == null) { throw new InvalidOperationException("Cannot commit null transaction"); }

            Transaction.Commit();
        }

        public SqliteConnection Connection { get; private set; }
        public SqliteTransaction Transaction { get; private set; }

        private async Task<IFactoryCreator> CreateReadWrite(IsolationLevel isolationLevel)
        {
            Connection = new SqliteConnection(_connectionString);
            await Connection.OpenAsync();
            Transaction = Connection.BeginTransaction(isolationLevel);
            return this;
        }

        private async Task<IFactoryCreator> CreateReadOnly()
        {
            Connection = new SqliteConnection(_connectionString);
            await Connection.OpenAsync();
            Transaction = null;
            return this;
        }

        public void Dispose()
        {
            Connection?.Dispose();
            Transaction?.Dispose();
        }
    }
}

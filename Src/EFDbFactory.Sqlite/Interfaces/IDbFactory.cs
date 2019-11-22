using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EFDbFactory.Sqlite
{
    public interface IDbFactory : IDisposable
    {
        /// <summary>
        /// Create a Connection with Transaction level, If Factory is created with InMemory Flag transactions will not be used.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        Task<IDbFactory> CreateTransactional(System.Data.IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Create a factory with no transaction
        /// </summary>
        /// <returns></returns>
        Task<IDbFactory> CreateReadOnly();

        /// <summary>
        /// Get your context with porvided sql connection and with transaction if factory is transactional and non transaction if the factory is pointing to InMemory Database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T FactoryFor<T>() where T : CommonDbContext;

        /// <summary>
        /// commit transaction when you have done your work. if there is an error in your code the transaction will not be committed. throw InvalidOperationException if the factory is created with no transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void CommitTransaction();

        /// <summary>
        /// return a sqlite connection when factory is being created.
        /// </summary>
        SqliteConnection Connection { get; }

        /// <summary>
        /// return sqlite transaction which is being initialized with factory creation
        /// </summary>
        SqliteTransaction Transaction { get; }
    }
}

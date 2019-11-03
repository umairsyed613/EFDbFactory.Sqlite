using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EFDbFactory.Sqlite
{
    public interface IFactoryCreator : IDisposable
    {
        /// <summary>
        /// Create a Connection with Transaction level
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        Task<IFactoryCreator> Create(System.Data.IsolationLevel isolationLevel);

        /// <summary>
        /// Create a factory with no transaction
        /// </summary>
        /// <returns></returns>
        Task<IFactoryCreator> Create();

        /// <summary>
        /// Get Database factory for your context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDbFactory<T> FactoryFor<T>() where T : CommonDbContext;

        /// <summary>
        /// commit transaction when you have done your work. if there is an error in your code the transaction will not be committed. throw InvalidOperationException if the factory is created with no transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void CommitTransaction();

        /// <summary>
        /// return a sql connection when factory is being created.
        /// </summary>
        SqliteConnection Connection { get; }

        /// <summary>
        /// return sql transaction which is being initialized with factory creation
        /// </summary>
        SqliteTransaction Transaction { get; }
    }
}

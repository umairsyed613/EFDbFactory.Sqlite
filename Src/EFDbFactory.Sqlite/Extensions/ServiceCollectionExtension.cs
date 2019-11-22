using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFDbFactory.Sqlite.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Add DB Factory with provided Connection string
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString));

            return services;
        }

        /// <summary>
        /// Add DB Factory with provided connection string ILoggerFactory for logging all your sql queries.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="enableSensitiveDataLogging"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString, ILoggerFactory loggerFactory, bool enableSensitiveDataLogging = false)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString, loggerFactory, enableSensitiveDataLogging));

            return services;
        }
        /// <summary>
        /// Add DB Factory with provided Sqlite Database connection string for In Memory Database with InMemory flag.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="inMemory"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString, bool inMemory)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString, inMemory));

            return services;
        }

        /// <summary>
        /// Add DB Factory with provided connection string ILoggerFactory for logging all your sql queries. InMemory Flag used for using Database in memory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="enableSensitiveDataLogging"></param>
        /// <param name="inMemory"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString, ILoggerFactory loggerFactory, bool enableSensitiveDataLogging, bool inMemory)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString, loggerFactory, enableSensitiveDataLogging, inMemory));

            return services;
        }
    }
}

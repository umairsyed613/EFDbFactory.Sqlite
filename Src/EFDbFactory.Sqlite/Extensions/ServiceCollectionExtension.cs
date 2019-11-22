using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFDbFactory.Sqlite.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString));

            return services;
        }

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

        public static IServiceCollection AddEfDbFactory(this IServiceCollection services, string connectionString, bool inMemory)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            services.AddSingleton<IDbFactory, DbFactory>(options => new DbFactory(connectionString, inMemory));

            return services;
        }

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

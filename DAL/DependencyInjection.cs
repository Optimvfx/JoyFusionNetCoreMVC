using Laraue.EfCoreTriggers.SqlLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection
            services, string connection, LogLevel logLevel = LogLevel.Information)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connection);
                options.UseSqlLiteTriggers();
                
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter(level => level == logLevel)));
            }, ServiceLifetime.Transient);

            return services;
        }
    }
}

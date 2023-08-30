using Laraue.EfCoreTriggers.SqlLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection
            services, string connection)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connection);
                options.UseSqlLiteTriggers();
            });

            return services;
        }
    }
}

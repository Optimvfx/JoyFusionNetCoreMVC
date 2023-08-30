using BLL.AutoMapper.Profiles;
using BLL.Services;
using BLL.Services.TimeService;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection
        services, ITimeService? customTimeService = null)
    {
        if(customTimeService == null)
            services.AddScoped<ITimeService, StandartTimeService>();
        else 
            services.AddScoped<ITimeService>((s) => customTimeService);
        
        services.AddScoped<UserService>();
        services.AddScoped<PostService>();
        services.AddAutoMapper(typeof(BaseAutoMapperProfile).Assembly);
        services.AddMemoryCache();
        
        return services;
    }
}

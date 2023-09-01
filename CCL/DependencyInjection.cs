using BLL.AutoMapper.Profiles;
using BLL.Services;
using CCL.ControllersLogic;
using CCL.ControllersLogic.Config;
using Common.Extensions;
using Common.Models;

namespace CCL;

public static class DependencyInjection
{
    public static IServiceCollection AddControllersLogic(this IServiceCollection
        services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        services.AddControllersLogicConfigs();
        
        services.AddWithCustomLifeTime<AuthControllerLogic>(serviceLifetime);
        services.AddWithCustomLifeTime<PostControllerLogic>(serviceLifetime);
        services.AddWithCustomLifeTime<ReactionsControllerLogic>(serviceLifetime);
        
        return services;
    }
    
    private static IServiceCollection AddControllersLogicConfigs(this IServiceCollection
        services)
    {
        services.AddTransient((s) => new PostControllersLogicConfig(5,
            new WidthHeightRange(new ValueRange<int>(100, 2000),
                new ValueRange<int>(100, 2000))));
        
        return services;
    }
}

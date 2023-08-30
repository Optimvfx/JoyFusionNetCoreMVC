using BLL.AutoMapper.Profiles;
using BLL.Services;
using CCL.ControllersLogic;
using CCL.ControllersLogic.Config;

namespace CCL;

public static class DependencyInjection
{
    public static IServiceCollection AddControllersLogic(this IServiceCollection
        services)
    {
        services.AddControllersLogicConfigs();
        
        services.AddScoped<AuthControllerLogic>();
        services.AddScoped<PostControllerLogic>();
        
        return services;
    }
    
    private static IServiceCollection AddControllersLogicConfigs(this IServiceCollection
        services)
    {
        services.AddTransient((s) => new PostControllersLogicConfig(5));
        
        return services;
    }
}

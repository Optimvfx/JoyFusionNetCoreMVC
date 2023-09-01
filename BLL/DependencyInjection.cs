using BLL.AutoMapper.Profiles;
using BLL.Services;
using BLL.Services.ImageService;
using BLL.Services.TimeService;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BLL;

public static class DependencyInjection
{
    private static readonly Func<IServiceProvider, ITimeService> _getStandartTimeService =
        (s) => new StandartTimeService();
    
    private static readonly Func<IServiceProvider, IImageService> _getStandartImageService = 
        (s) => new StandartImageService();

    public static IServiceCollection AddServices(this IServiceCollection services, 
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        services.AddWithCustomLifeTime<UserService>(serviceLifetime);
        services.AddWithCustomLifeTime<PostService>(serviceLifetime);
        services.AddWithCustomLifeTime<ReactionsService>(serviceLifetime);
        
        services.AddAutoMapper(typeof(BaseAutoMapperProfile).Assembly);
        services.AddMemoryCache();
        
        return services;
    }
    
    public static IServiceCollection AddCustumizableServices(this IServiceCollection services,
        Func<IServiceProvider, ITimeService> getCustomTimeService = null,
        Func<IServiceProvider, IImageService> getCustomImageService = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        getCustomTimeService = getCustomTimeService ?? _getStandartTimeService;
        getCustomImageService = getCustomImageService ?? _getStandartImageService;
        
        services.AddWithCustomLifeTime<ITimeService>(serviceLifetime, s => getCustomTimeService(s));
        services.AddWithCustomLifeTime<IImageService>(serviceLifetime, s => getCustomImageService(s));

        return services;
    }
}

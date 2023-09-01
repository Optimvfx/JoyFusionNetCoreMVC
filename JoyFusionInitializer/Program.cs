using System;
using System.Data.Common;
using BLL;
using BLL.Services.ImageService;
using BLL.Services.TimeService;
using CCL;
using Common.Helpers;
using Common.Models;
using DAL;
using JoyFusionInitializer.Models;
using JoyFusionInitializer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace JoyFusionInitializer;

public class Program
{
    public static void Main()
    {
        var configuration = WebApplication.CreateBuilder().Configuration;
        configuration.AddJsonFile("configuration.json");

        var stepTimeServiceConfig =
            configuration.GetSection("StepTimeServiceConfig").Get<StepTimeServiceConfig>();

        var connection = configuration.GetConnectionString("DefaultConnectionString") ??
                         throw new NullReferenceException();

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });
        
        services.AddDatabase(connection, LogLevel.Error);

        services.AddCustumizableServices(
            (s) =>  GetTimeService(stepTimeServiceConfig));

        services.AddServices();

        services.AddControllersLogic();

        services.AddTransient<RandomImageGeneratorService.Config>((s) => new RandomImageGeneratorService.Config(
            new []{300, 400, 500, 600, 700, 800, 900}, new []{200, 300, 400, 500, 600, 700}, 5));

        services.AddTransient<RandomImageGeneratorService>();
        services.AddTransient<InitializeConfigModelGenerator>();
        services.AddTransient<AppInitializer>();
        
        services.AddTransient<StopwatchHelper>();
        
        var servicesProvider = services.BuildServiceProvider();
        
        ClearDatabase(servicesProvider.GetService<ApplicationDbContext>());

        var stopWatch = servicesProvider.GetService<StopwatchHelper>();

        InitializeConfigModelGenerator configGenerator = null;

        stopWatch.LogActionWorkTime( "Config generator images creation", LogLevel.Warning, () =>
        {
            configGenerator = servicesProvider.GetService<InitializeConfigModelGenerator>();
        });

        InitializeConfigModel initializeConfig = null;

        stopWatch.LogActionWorkTime( "Config Generation", LogLevel.Warning, () =>
        {
            initializeConfig = configGenerator.Generete(100, 30, 3);
        });

        stopWatch.LogActionWorkTime("App Initialization", LogLevel.Warning,  () =>
        {
            var initializer = new AppInitializer(servicesProvider);
            initializer.Initialize(initializeConfig, new Procent(0.2f), new Procent(0.45f)).Wait();
        });
        
        Console.ReadLine();
    }

    private static ITimeService GetTimeService(StepTimeServiceConfig config)
    {
       return new StepTimeService(
            config.StartTimePosition,
            () =>
            {
                var rand = new Random();
                int randomMinutes = rand.Next(0, (int)config.MaxMinuteTimeStep);
                return new TimeSpan(0, 0, randomMinutes, 0);
            });
    }
    
    private static void ClearDatabase(ApplicationDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
}
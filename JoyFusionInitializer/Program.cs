using System;
using BLL;
using CCL;
using DAL;
using JoyFusionInitializer.Models;
using JoyFusionInitializer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JoyFusionInitializer;

public class Program
{
    public static void Main()
    {
        var configuration = WebApplication.CreateBuilder().Configuration;
        configuration.AddJsonFile("configuration.json");
        
        var stepTimeServiceConfig =
            configuration.GetSection("StepTimeServiceConfig").Get<StepTimeServiceConfig>();

        var initializeConfig = configuration.GetSection("InitializeConfig").Get<InitializeConfigModel>() ??
                               throw new NullReferenceException();

        var connection =configuration.GetConnectionString("DefaultConnectionString") ??
                        throw new NullReferenceException();
        
        var services = new ServiceCollection();
        services.AddDatabase(connection);

        services.AddServices(new StepTimeService(
            stepTimeServiceConfig.StartTimePosition,
            () =>
            {
                var rand = new Random();
                int randomMinutes = rand.Next(0, (int)stepTimeServiceConfig.MaxMinuteTimeStep);
                return new TimeSpan(0, 0, randomMinutes, 0);
            }));

       services.AddControllersLogic();

       var servicesProvider = services.BuildServiceProvider();
       
       servicesProvider.Initialize(initializeConfig);
    }
}
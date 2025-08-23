using Microsoft.Extensions.DependencyInjection;
using PlantTracker.Application.Services;
using PlantTracker.Core.Interfaces;

namespace PlantTracker.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        return services.AddTransient<IPlantService, PlantService>();
    }
}
using Microsoft.Extensions.DependencyInjection;
using PlantTrackerCleanArchitectureApi.Application.Services;

namespace PlantTrackerCleanArchitectureApi.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        return services.AddTransient<IPlantService, PlantService>();
    }
}
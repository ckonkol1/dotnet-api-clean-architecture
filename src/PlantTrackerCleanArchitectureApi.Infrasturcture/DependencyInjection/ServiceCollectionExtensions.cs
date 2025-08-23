using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantTrackerCleanArchitectureApi.Infrastructure.Configurations;
using PlantTrackerCleanArchitectureApi.Infrastructure.Repositories;

namespace PlantTrackerCleanArchitectureApi.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var awsConfig = new AwsConfig();
        config.GetSection("Aws").Bind(awsConfig);

        return services.AddSingleton<IAmazonDynamoDB>(provider =>
            {
                var awsDynamoDbConfig = new AmazonDynamoDBConfig
                {
                    Timeout = TimeSpan.FromSeconds(10),
                    ConnectTimeout = TimeSpan.FromSeconds(10),
                    MaxErrorRetry = 2,
                    RetryMode = RequestRetryMode.Standard
                };

                if (!string.IsNullOrEmpty(awsConfig.Region))
                {
                    awsDynamoDbConfig.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig.Region);
                }

                // For local DynamoDB
                if (!string.IsNullOrEmpty(awsConfig.ServiceUrl))
                {
                    awsDynamoDbConfig.ServiceURL = awsConfig.ServiceUrl;
                }

                if (!string.IsNullOrEmpty(awsConfig.AccessKey) && !string.IsNullOrEmpty(awsConfig.SecretKey))
                {
                    return new AmazonDynamoDBClient(awsConfig.AccessKey, awsConfig.SecretKey, awsDynamoDbConfig);
                }

                return new AmazonDynamoDBClient(awsDynamoDbConfig);
            })
            .AddScoped<IDynamoDBContext, DynamoDBContext>()
            .AddScoped<IPlantRepository, PlantRepository>()
            .AddSingleton<TimeProvider>(TimeProvider.System);
    }
}
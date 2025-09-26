using Microsoft.Extensions.DependencyInjection;

namespace EnergiDataService.Client.Extensions;

/// <summary>
/// Extension methods for adding EnergiDataServiceClient to the service collection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the EnergiDataServiceClient to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEnergiDataServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<EnergiDataServiceClient>();
        return services;
    }

    /// <summary>
    /// Adds the EnergiDataServiceClient to the service collection with custom HTTP client configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureClient">Action to configure the HTTP client</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEnergiDataServiceClient(
        this IServiceCollection services,
        Action<HttpClient> configureClient)
    {
        services.AddHttpClient<EnergiDataServiceClient>(configureClient);
        return services;
    }
}
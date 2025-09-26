using EnergiDataService.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EnergiDataService.Client.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEnergiDataServiceClient_RegistersClientCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEnergiDataServiceClient();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<EnergiDataServiceClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public void AddEnergiDataServiceClient_WithConfiguration_RegistersClientCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationCalled = false;

        // Act
        services.AddEnergiDataServiceClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            configurationCalled = true;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<EnergiDataServiceClient>();
        Assert.NotNull(client);
        Assert.True(configurationCalled);
    }

    [Fact]
    public void AddEnergiDataServiceClient_RegistersHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEnergiDataServiceClient();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.NotNull(httpClientFactory);
    }
}
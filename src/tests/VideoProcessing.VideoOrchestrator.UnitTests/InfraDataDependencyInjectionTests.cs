using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting;
using VideoProcessing.VideoOrchestrator.Infra.Data;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

/// <summary>
/// Garante que AddOrchestratorData registra os serviços sem falhar (config válida).
/// </summary>
[Collection("EnvVars")]
public sealed class InfraDataDependencyInjectionTests
{
    private static IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:BASE_URL"] = "https://api.test",
                ["VIDEO_MANAGEMENT_API:TIMEOUT_SECONDS"] = "30",
                ["M2M_AUTH:TOKEN_ENDPOINT"] = "https://auth.test/token",
                ["M2M_AUTH:CLIENT_ID"] = "client",
                ["M2M_AUTH:CLIENT_SECRET"] = "secret",
                ["STEP_FUNCTION:STATE_MACHINE_ARN"] = "arn:aws:states:us-east-1:123456789012:stateMachine:Test"
            })
            .Build();
    }

    [Fact]
    [Trait("Category", "EnvVars")]
    public void AddOrchestratorData_WhenConfigValid_RegistersServicesAndReturnsSameCollection()
    {
        var config = BuildConfig();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddOrchestratorConfiguration(config);

        var result = services.AddOrchestratorData(config);

        result.Should().BeSameAs(services);
        var provider = services.BuildServiceProvider(validateScopes: true);
        provider.Should().NotBeNull();
    }
}

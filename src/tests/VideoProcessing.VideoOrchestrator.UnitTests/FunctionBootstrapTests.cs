using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

/// <summary>
/// Testes de bootstrap (config + DI) — chaves no formato do provider de env vars (seção__chave).
/// </summary>
[Collection("EnvVars")]
public sealed class FunctionBootstrapTests
{
    /// <summary>
    /// Config com o mesmo formato de chaves que AddEnvironmentVariables() produz (VIDEO_MANAGEMENT_API:BASE_URL).
    /// </summary>
    /// <summary>Chaves no formato env (seção em maiúsculas, subchave com underscore).</summary>
    private static IConfiguration BuildConfigWithEnvVarKeys()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:BASE_URL"] = "https://api.internal",
                ["VIDEO_MANAGEMENT_API:TIMEOUT_SECONDS"] = "30",
                ["M2M_AUTH:TOKEN_ENDPOINT"] = "https://auth.example.com/token",
                ["M2M_AUTH:CLIENT_ID"] = "client-id",
                ["M2M_AUTH:CLIENT_SECRET"] = "secret",
                ["STEP_FUNCTION:STATE_MACHINE_ARN"] = "arn:aws:states:us-east-1:123456789012:stateMachine:Test"
            })
            .Build();
    }

    [Fact]
    [Trait("Category", "EnvVars")]
    public void Bootstrap_WhenAllRequiredConfigPresent_ResolvesOptions()
    {
        var config = BuildConfigWithEnvVarKeys();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddOrchestratorConfiguration(config);
        var provider = services.BuildServiceProvider(validateScopes: true);

        var videoOptions = provider.GetRequiredService<IOptions<VideoManagementApiOptions>>().Value;
        videoOptions.BaseUrl.Should().Be("https://api.internal");
        videoOptions.TimeoutSeconds.Should().Be(30);

        var m2mOptions = provider.GetRequiredService<IOptions<M2MAuthOptions>>().Value;
        m2mOptions.TokenEndpoint.Should().Be("https://auth.example.com/token");
        m2mOptions.ClientId.Should().Be("client-id");

        var stepOptions = provider.GetRequiredService<IOptions<StepFunctionOptions>>().Value;
        stepOptions.StateMachineArn.Should().Be("arn:aws:states:us-east-1:123456789012:stateMachine:Test");
    }

    [Fact]
    [Trait("Category", "EnvVars")]
    public void Bootstrap_WhenRequiredConfigMissing_ThrowsOptionsValidationException()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:BASE_URL"] = "https://api.internal",
                ["M2M_AUTH:TOKEN_ENDPOINT"] = "https://auth.example.com/token",
                ["M2M_AUTH:CLIENT_ID"] = "client-id",
                ["M2M_AUTH:CLIENT_SECRET"] = "secret"
                // STEP_FUNCTION:STATE_MACHINE_ARN ausente
            })
            .Build();
        var services = new ServiceCollection();
        services.AddOrchestratorConfiguration(config);
        var provider = services.BuildServiceProvider(validateScopes: true);

        var act = () => _ = provider.GetRequiredService<IOptions<StepFunctionOptions>>().Value;
        act.Should().Throw<OptionsValidationException>();
    }
}

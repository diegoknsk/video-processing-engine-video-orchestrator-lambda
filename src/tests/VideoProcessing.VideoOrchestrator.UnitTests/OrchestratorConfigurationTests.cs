using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class OrchestratorConfigurationTests
{
    [Fact]
    public void VideoManagementApiOptions_WhenAllFieldsPresent_BindsCorrectly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:BASE_URL"] = "https://api.internal",
                ["VIDEO_MANAGEMENT_API:TIMEOUT_SECONDS"] = "30"
            })
            .Build();

        var options = config.GetSection("VIDEO_MANAGEMENT_API").Get<VideoManagementApiOptions>();

        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://api.internal");
        options.TimeoutSeconds.Should().Be(30);
    }

    [Fact]
    public void VideoManagementApiOptions_WhenTimeoutOmitted_DefaultsTo30()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:BASE_URL"] = "https://api.internal"
            })
            .Build();

        var options = config.GetSection("VIDEO_MANAGEMENT_API").Get<VideoManagementApiOptions>();

        options.Should().NotBeNull();
        options!.TimeoutSeconds.Should().Be(30);
    }

    [Fact]
    public void M2MAuthOptions_WhenAllFieldsPresent_BindsCorrectly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["M2M_AUTH:TOKEN_ENDPOINT"] = "https://auth.example.com/token",
                ["M2M_AUTH:CLIENT_ID"] = "client-id",
                ["M2M_AUTH:CLIENT_SECRET"] = "secret-value"
            })
            .Build();

        var options = config.GetSection("M2M_AUTH").Get<M2MAuthOptions>();

        options.Should().NotBeNull();
        options!.TokenEndpoint.Should().Be("https://auth.example.com/token");
        options.ClientId.Should().Be("client-id");
        options.ClientSecret.Should().Be("secret-value");
    }

    [Fact]
    public void StepFunctionOptions_WhenArnPresent_BindsCorrectly()
    {
        var arn = "arn:aws:states:us-east-1:123456789012:stateMachine:MyStateMachine";
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["STEP_FUNCTION:STATE_MACHINE_ARN"] = arn
            })
            .Build();

        var options = config.GetSection("STEP_FUNCTION").Get<StepFunctionOptions>();

        options.Should().NotBeNull();
        options!.StateMachineArn.Should().Be(arn);
    }

    [Fact]
    public void M2MAuthOptions_WhenClientSecretMissing_ValidationFails()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["M2M_AUTH:TOKEN_ENDPOINT"] = "https://auth.example.com/token",
                ["M2M_AUTH:CLIENT_ID"] = "client-id"
                // CLIENT_SECRET ausente (required)
            })
            .Build();

        var services = new ServiceCollection();
        services.AddOptions<M2MAuthOptions>()
            .Bind(config.GetSection("M2M_AUTH"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<M2MAuthOptions>>();

        var act = () => _ = monitor.CurrentValue;

        act.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void StepFunctionOptions_WhenStateMachineArnMissing_ValidationFails()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var services = new ServiceCollection();
        services.AddOptions<StepFunctionOptions>()
            .Bind(config.GetSection("STEP_FUNCTION"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<StepFunctionOptions>>();

        var act = () => _ = monitor.CurrentValue;

        act.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void VideoManagementApiOptions_WhenBaseUrlMissing_ValidationFails()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VIDEO_MANAGEMENT_API:TIMEOUT_SECONDS"] = "60"
                // BASE_URL ausente (required)
            })
            .Build();

        var services = new ServiceCollection();
        services.AddOptions<VideoManagementApiOptions>()
            .Bind(config.GetSection("VIDEO_MANAGEMENT_API"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<VideoManagementApiOptions>>();

        var act = () => _ = monitor.CurrentValue;

        act.Should().Throw<OptionsValidationException>();
    }

}

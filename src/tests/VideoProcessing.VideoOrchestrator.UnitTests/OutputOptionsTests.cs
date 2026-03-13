using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Application.Options;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class OutputOptionsTests
{
    [Fact]
    public void Bind_WithEmptyConfiguration_ReturnsDefaultValues()
    {
        var config = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.Configure<OutputOptions>(config.GetSection(OutputOptions.SectionName));
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<OutputOptions>>().Value;

        options.FramesBucket.Should().Be("video-processing-engine-dev-images");
        options.ManifestBucket.Should().Be("video-processing-engine-dev-images");
        options.ZipBucket.Should().Be("video-processing-engine-dev-zip");
        options.FrameIntervalSec.Should().Be(1);
        options.OrdenaAutomaticamente.Should().BeTrue();
    }

    [Fact]
    public void Bind_WithExplicitValues_BindsCorrectly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OUTPUT:FramesBucket"] = "my-frames",
                ["OUTPUT:ManifestBucket"] = "my-manifest",
                ["OUTPUT:ZipBucket"] = "my-zip",
                ["OUTPUT:FrameIntervalSec"] = "3",
                ["OUTPUT:OrdenaAutomaticamente"] = "false"
            })
            .Build();
        var options = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(options);

        options.FramesBucket.Should().Be("my-frames");
        options.ManifestBucket.Should().Be("my-manifest");
        options.ZipBucket.Should().Be("my-zip");
        options.FrameIntervalSec.Should().Be(3);
        options.OrdenaAutomaticamente.Should().BeFalse();
    }
}

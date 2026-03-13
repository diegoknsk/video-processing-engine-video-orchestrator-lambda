using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Application.Parsers;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class S3KeyParserTests
{
    [Fact]
    public void Parse_ValidKey_ReturnsUserIdAndVideoId()
    {
        var (userId, videoId) = S3KeyParser.Parse("videos/user-1/video-2/original");

        userId.Should().Be("user-1");
        videoId.Should().Be("video-2");
    }

    [Fact]
    public void Parse_KeyWithoutExpectedPrefix_ThrowsFormatException()
    {
        var act = () => S3KeyParser.Parse("other/user-1/video-2/original");

        act.Should().Throw<FormatException>()
            .WithMessage("*videos/*");
    }

    [Fact]
    public void Parse_KeyWithoutExpectedSuffix_ThrowsFormatException()
    {
        var act = () => S3KeyParser.Parse("videos/user-1/video-2/final");

        act.Should().Throw<FormatException>()
            .WithMessage("*original*");
    }

    [Fact]
    public void Parse_KeyWithTooManySegments_ThrowsFormatException()
    {
        var act = () => S3KeyParser.Parse("videos/user-1/video-2/extra/original");

        act.Should().Throw<FormatException>()
            .WithMessage("*two path segments*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_NullOrEmptyKey_ThrowsArgumentException(string? key)
    {
        var act = () => S3KeyParser.Parse(key!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("key");
    }

    [Fact]
    public void Parse_RealS3KeyFromPromptSample_ReturnsCorrectIds()
    {
        var key = "videos/0468f438-1234-5678-abcd-111111111111/16c39167-1234-5678-abcd-222222222222/original";
        var (userId, videoId) = S3KeyParser.Parse(key);

        userId.Should().Be("0468f438-1234-5678-abcd-111111111111");
        videoId.Should().Be("16c39167-1234-5678-abcd-222222222222");
    }
}

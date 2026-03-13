using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Application.UseCases;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class FetchVideoDetailsUseCaseTests
{
    private readonly Mock<IM2MTokenService> _tokenServiceMock = new();
    private readonly Mock<IVideoManagementClient> _videoClientMock = new();
    private readonly Mock<ILogger<FetchVideoDetailsUseCase>> _loggerMock = new();

    [Fact]
    public async Task ExecuteAsync_HappyPath_ReturnsMappedVideoDetails()
    {
        var sut = new FetchVideoDetailsUseCase(_tokenServiceMock.Object, _videoClientMock.Object, _loggerMock.Object);
        _tokenServiceMock.Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("token");
        var expected = new VideoDetails("v1", "u1", "Title", "Status", "videos/u1/v1/original", "", "User", "user@x.com", 0, 0, 1);
        _videoClientMock
            .Setup(x => x.GetVideoDetailsAsync("u1", "v1", "token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await sut.ExecuteAsync("videos/u1/v1/original");

        result.Should().Be(expected);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidS3Key_PropagatesFormatException()
    {
        var sut = new FetchVideoDetailsUseCase(_tokenServiceMock.Object, _videoClientMock.Object, _loggerMock.Object);

        var act = () => sut.ExecuteAsync("invalid-key");

        await act.Should().ThrowAsync<FormatException>();
    }

    [Fact]
    public async Task ExecuteAsync_TokenServiceFails_PropagatesException()
    {
        var sut = new FetchVideoDetailsUseCase(_tokenServiceMock.Object, _videoClientMock.Object, _loggerMock.Object);
        _tokenServiceMock.Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Auth failed"));

        var act = () => sut.ExecuteAsync("videos/u1/v1/original");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Auth failed");
    }

    [Fact]
    public async Task ExecuteAsync_VideoManagementFails_PropagatesException()
    {
        var sut = new FetchVideoDetailsUseCase(_tokenServiceMock.Object, _videoClientMock.Object, _loggerMock.Object);
        _tokenServiceMock.Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("token");
        _videoClientMock
            .Setup(x => x.GetVideoDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new VideoProcessing.VideoOrchestrator.Domain.Exceptions.NotFoundException("Not found"));

        var act = () => sut.ExecuteAsync("videos/u1/v1/original");

        await act.Should().ThrowAsync<VideoProcessing.VideoOrchestrator.Domain.Exceptions.NotFoundException>();
    }
}

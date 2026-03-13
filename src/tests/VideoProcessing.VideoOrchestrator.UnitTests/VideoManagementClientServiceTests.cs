using System.Net;
using FluentAssertions;
using Moq;
using Refit;
using VideoProcessing.VideoOrchestrator.Domain.Exceptions;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class VideoManagementClientServiceTests
{
    private readonly Mock<IVideoManagementApi> _apiMock = new();

    [Fact]
    public async Task GetVideoDetailsAsync_WhenApiReturnsVideo_MapsToVideoDetails()
    {
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync(
                "user-1",
                "video-2",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VideoManagementApiResponse
            {
                Success = true,
                Data = new VideoManagementVideoData
                {
                    VideoId = "video-2",
                    UserId = "user-1",
                    UserEmail = "john@example.com",
                    OriginalFileName = "My Video",
                    Status = 1,
                    StatusDescription = "Uploaded",
                    S3KeyVideo = "videos/user-1/video-2/original",
                    S3BucketVideo = "my-bucket"
                }
            });

        var result = await sut.GetVideoDetailsAsync("user-1", "video-2", "token-123");

        result.Should().BeEquivalentTo(new VideoDetails(
            "video-2",
            "user-1",
            "My Video",
            "Uploaded",
            "videos/user-1/video-2/original",
            "my-bucket",
            "",
            "john@example.com",
            0,
            0,
            1));
    }

    [Fact]
    public async Task GetVideoDetailsAsync_When404_ThrowsNotFoundException()
    {
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(await ApiException.Create(
                new HttpRequestMessage(),
                HttpMethod.Get,
                new HttpResponseMessage(HttpStatusCode.NotFound),
                null!));

        var act = () => sut.GetVideoDetailsAsync("user-1", "video-2", "token");

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*userId=user-1*videoId=video-2*");
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WhenBearerTokenFormatted_PassesCorrectHeader()
    {
        string? capturedAuth = null;
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync("u", "v", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, string, CancellationToken>((_, _, auth, _) => capturedAuth = auth)
            .ReturnsAsync(new VideoManagementApiResponse
            {
                Data = new VideoManagementVideoData { VideoId = "v", UserId = "u", S3KeyVideo = "", S3BucketVideo = "" }
            });

        await sut.GetVideoDetailsAsync("u", "v", "my-token");

        capturedAuth.Should().Be("Bearer my-token");
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WhenApiReturnsParallelChunksZero_MapsToParallelChunksOne()
    {
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VideoManagementApiResponse
            {
                Success = true,
                Data = new VideoManagementVideoData
                {
                    VideoId = "vid",
                    UserId = "usr",
                    UserEmail = "e@x.com",
                    OriginalFileName = "F",
                    StatusDescription = "Uploaded",
                    S3KeyVideo = "k",
                    S3BucketVideo = "b",
                    DurationSec = 30,
                    FrameIntervalSec = 5,
                    ParallelChunks = 0
                }
            });

        var result = await sut.GetVideoDetailsAsync("usr", "vid", "token");

        result.ParallelChunks.Should().Be(1);
        result.DurationSec.Should().Be(30);
        result.FrameIntervalSec.Should().Be(5);
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WhenApiReturns5xx_ThrowsExternalServiceException()
    {
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(await ApiException.Create(
                new HttpRequestMessage(),
                HttpMethod.Get,
                new HttpResponseMessage(HttpStatusCode.InternalServerError),
                null!));

        var act = () => sut.GetVideoDetailsAsync("user-1", "video-2", "token");

        await act.Should().ThrowAsync<ExternalServiceException>()
            .WithMessage("*Video Management API error*InternalServerError*");
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WhenApiReturnsDurationAndChunks_MapsOneToOne()
    {
        var sut = new VideoManagementClientService(_apiMock.Object);
        _apiMock
            .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VideoManagementApiResponse
            {
                Success = true,
                Data = new VideoManagementVideoData
                {
                    VideoId = "v",
                    UserId = "u",
                    UserEmail = "e@x.com",
                    OriginalFileName = "F",
                    StatusDescription = "Uploaded",
                    S3KeyVideo = "k",
                    S3BucketVideo = "b",
                    DurationSec = 45,
                    FrameIntervalSec = 5,
                    ParallelChunks = 3
                }
            });

        var result = await sut.GetVideoDetailsAsync("u", "v", "token");

        result.DurationSec.Should().Be(45);
        result.FrameIntervalSec.Should().Be(5);
        result.ParallelChunks.Should().Be(3);
    }
}

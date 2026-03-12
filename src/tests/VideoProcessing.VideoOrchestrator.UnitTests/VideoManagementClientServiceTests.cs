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
            .ReturnsAsync(new VideoManagementVideoResponse
            {
                Id = "video-2",
                UserId = "user-1",
                Title = "My Video",
                Status = "Uploaded",
                S3Key = "videos/user-1/video-2/original",
                User = new VideoManagementUserInfo { Name = "John", Email = "john@example.com" }
            });

        var result = await sut.GetVideoDetailsAsync("user-1", "video-2", "token-123");

        result.Should().BeEquivalentTo(new VideoDetails(
            "video-2",
            "user-1",
            "My Video",
            "Uploaded",
            "videos/user-1/video-2/original",
            "",
            "John",
            "john@example.com"));
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
            .ReturnsAsync(new VideoManagementVideoResponse { Id = "v", UserId = "u", Title = "", Status = "", S3Key = "" });

        await sut.GetVideoDetailsAsync("u", "v", "my-token");

        capturedAuth.Should().Be("Bearer my-token");
    }
}

using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

/// <summary>
/// Cobertura dos DTOs da API Video Management (contratos de serialização).
/// </summary>
public sealed class VideoManagementUserInfoTests
{
    [Fact]
    public void VideoManagementUserInfo_Properties_ReflectInitValues()
    {
        var dto = new VideoManagementUserInfo
        {
            Name = "Jane",
            Email = "jane@example.com"
        };

        dto.Name.Should().Be("Jane");
        dto.Email.Should().Be("jane@example.com");
    }
}

public sealed class VideoManagementVideoResponseTests
{
    [Fact]
    public void VideoManagementVideoResponse_Properties_ReflectInitValues()
    {
        var user = new VideoManagementUserInfo { Name = "Bob", Email = "bob@example.com" };
        var dto = new VideoManagementVideoResponse
        {
            Id = "vid-1",
            UserId = "user-1",
            Title = "My Video",
            Status = "Uploaded",
            S3Key = "videos/key",
            User = user
        };

        dto.Id.Should().Be("vid-1");
        dto.UserId.Should().Be("user-1");
        dto.Title.Should().Be("My Video");
        dto.Status.Should().Be("Uploaded");
        dto.S3Key.Should().Be("videos/key");
        dto.User.Should().BeSameAs(user);
    }
}

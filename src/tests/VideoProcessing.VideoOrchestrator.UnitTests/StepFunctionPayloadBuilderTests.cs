using System.Text.Json;
using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Application.Builders;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class StepFunctionPayloadBuilderTests
{
    private static VideoDetails CreateDetails(string videoId = "v1", string userId = "u1", string s3Bucket = "my-bucket") =>
        new(videoId, userId, "Meu Vídeo", "Uploaded", $"videos/{userId}/{videoId}/original", s3Bucket, "João Silva", "joao@example.com");

    [Fact]
    public void Build_WithValidVideoDetails_ReturnsPayloadWithCorrectVideoId()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-123");

        payload.Video.VideoId.Should().Be("v1");
        payload.Video.UserId.Should().Be("u1");
        payload.ExecutionId.Should().Be("exec-123");
    }

    [Fact]
    public void Build_WithValidVideoDetails_DerivesOutputPrefixCorrectly()
    {
        var details = CreateDetails("vid-2", "usr-3");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-x");

        payload.Video.OutputPrefix.Should().Be("videos/usr-3/vid-2/frames/");
    }

    [Fact]
    public void Build_WithValidVideoDetails_DerivesZipOutputBucketCorrectly()
    {
        var details = CreateDetails(s3Bucket: "video-bucket-dev");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1");

        payload.Zip.OutputBucket.Should().Be("video-bucket-dev");
    }

    [Fact]
    public void Build_WithValidVideoDetails_DerivesZipOutputKeyCorrectly()
    {
        var details = CreateDetails("v2", "u2");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1");

        payload.Zip.OutputKey.Should().Be("videos/u2/v2/output.zip");
    }

    [Fact]
    public void Build_WithValidVideoDetails_ChunksContainsAtLeastOneItem()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1");

        payload.Video.Chunks.Should().NotBeEmpty();
        payload.Video.Chunks.Should().HaveCount(1);
    }

    [Fact]
    public void Build_WithValidVideoDetails_FirstChunkHasCorrectOutputPath()
    {
        var details = CreateDetails("vid-x", "usr-y");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1");

        payload.Video.Chunks[0].ChunkIndex.Should().Be(0);
        payload.Video.Chunks[0].OutputPath.Should().Be("videos/usr-y/vid-x/frames/chunk-0/");
    }

    [Fact]
    public void Build_WithValidVideoDetails_MapsUserNameAndEmail()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1");

        payload.User.Name.Should().Be("João Silva");
        payload.User.Email.Should().Be("joao@example.com");
    }

    [Fact]
    public void Build_PayloadSerializesToValidJson()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-123");
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        json.Should().Contain("\"executionId\":\"exec-123\"");
        json.Should().Contain("\"video\"");
        json.Should().Contain("\"chunks\"");
        json.Should().Contain("\"outputBucket\"");
        json.Should().Contain("\"outputKey\"");
        json.Should().Contain("\"user\"");
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"email\"");
    }
}

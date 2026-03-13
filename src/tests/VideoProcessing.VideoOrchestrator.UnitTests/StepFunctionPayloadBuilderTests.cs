using System.Text.Json;
using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Application.Builders;
using VideoProcessing.VideoOrchestrator.Application.Options;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class StepFunctionPayloadBuilderTests
{
    private static VideoDetails CreateDetails(string videoId = "v1", string userId = "u1", string s3Bucket = "my-bucket", int durationSec = 45, int frameIntervalSec = 0, int parallelChunks = 1) =>
        new(videoId, userId, "Meu Vídeo", "Uploaded", $"videos/{userId}/{videoId}/original", s3Bucket, "João Silva", "joao@example.com", durationSec, frameIntervalSec, parallelChunks);

    private static OutputOptions DefaultOptions() => new();

    [Fact]
    public void Build_WithValidVideoDetails_ReturnsPayloadWithCorrectVideoId()
    {
        var details = CreateDetails();
        var options = DefaultOptions();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-123", options);

        payload.Video.VideoId.Should().Be("v1");
        payload.Video.UserId.Should().Be("u1");
        payload.ExecutionId.Should().Be("exec-123");
    }

    [Fact]
    public void Build_WithValidVideoDetails_DerivesOutputPrefixCorrectly()
    {
        var details = CreateDetails("vid-2", "usr-3");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-x", DefaultOptions());

        payload.Video.OutputPrefix.Should().Be("videos/usr-3/vid-2/frames/");
    }

    [Fact]
    public void Build_WithDefaultOptions_ReturnsPayloadWithDefaultBuckets()
    {
        var details = CreateDetails();
        var options = DefaultOptions();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        payload.Output.FramesBucket.Should().Be("video-processing-engine-dev-images");
        payload.Output.ManifestBucket.Should().Be("video-processing-engine-dev-images");
        payload.Zip.OutputBucket.Should().Be("video-processing-engine-dev-zip");
        payload.Finalize.FramesBucket.Should().Be("video-processing-engine-dev-images");
        payload.Finalize.OutputBucket.Should().Be("video-processing-engine-dev-zip");
    }

    [Fact]
    public void Build_WithCustomOptions_ReturnsPayloadWithCustomBuckets()
    {
        var details = CreateDetails();
        var options = new OutputOptions
        {
            FramesBucket = "custom-frames",
            ManifestBucket = "custom-manifest",
            ZipBucket = "custom-zip"
        };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        payload.Output.FramesBucket.Should().Be("custom-frames");
        payload.Output.ManifestBucket.Should().Be("custom-manifest");
        payload.Zip.OutputBucket.Should().Be("custom-zip");
        payload.Finalize.FramesBucket.Should().Be("custom-frames");
        payload.Finalize.OutputBucket.Should().Be("custom-zip");
    }

    [Fact]
    public void Build_WithValidVideoDetails_DerivesZipOutputKeyCorrectly()
    {
        var details = CreateDetails("v2", "u2");
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Zip.OutputKey.Should().Be("videos/u2/v2/output.zip");
    }

    [Fact]
    public void Build_WithValidVideoDetails_ChunksContainsAtLeastOneItem()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().NotBeEmpty();
        payload.Video.Chunks.Should().HaveCount(1);
    }

    [Fact]
    public void Build_Chunk0_HasCorrectFields()
    {
        var details = CreateDetails("vid-x", "usr-y", durationSec: 45, parallelChunks: 1);
        var options = new OutputOptions { FrameIntervalSec = 2 };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        var chunk = payload.Video.Chunks[0];
        chunk.ChunkIndex.Should().Be(0);
        chunk.ChunkId.Should().Be("vid-x-chunk-0");
        chunk.StartSec.Should().Be(0);
        chunk.EndSec.Should().Be(45);
        chunk.IntervalSec.Should().Be(2);
        chunk.ManifestPrefix.Should().Be("videos/usr-y/vid-x/manifests/chunk-0/");
        chunk.FramesPrefix.Should().Be("videos/usr-y/vid-x/frames/chunk-0/");
    }

    [Fact]
    public void Build_FinalizeBlock_ContainsAllRequiredFields()
    {
        var details = CreateDetails("v1", "u1");
        var options = new OutputOptions { OrdenaAutomaticamente = false };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        payload.Finalize.FramesBucket.Should().Be("video-processing-engine-dev-images");
        payload.Finalize.FramesBasePrefix.Should().Be("videos/u1/v1/frames/");
        payload.Finalize.OutputBucket.Should().Be("video-processing-engine-dev-zip");
        payload.Finalize.VideoId.Should().Be("v1");
        payload.Finalize.OutputBasePrefix.Should().Be("u1/v1");
        payload.Finalize.OrdenaAutomaticamente.Should().BeFalse();
    }

    [Fact]
    public void Build_ContractVersion_Is_1_0()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.ContractVersion.Should().Be("1.0");
    }

    [Fact]
    public void Build_WithValidVideoDetails_MapsUserNameAndEmail()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.User.Name.Should().Be("João Silva");
        payload.User.Email.Should().Be("joao@example.com");
    }

    [Fact]
    public void Build_PayloadSerializesToValidJson()
    {
        var details = CreateDetails();
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-123", DefaultOptions());
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        json.Should().Contain("\"contractVersion\":\"1.0\"");
        json.Should().Contain("\"executionId\":\"exec-123\"");
        json.Should().Contain("\"video\"");
        json.Should().Contain("\"chunks\"");
        json.Should().Contain("\"output\"");
        json.Should().Contain("\"framesBucket\"");
        json.Should().Contain("\"manifestBucket\"");
        json.Should().Contain("\"outputBucket\"");
        json.Should().Contain("\"outputKey\"");
        json.Should().Contain("\"finalize\"");
        json.Should().Contain("\"user\"");
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"email\"");
    }

    [Fact]
    public void Build_DivisaoExata_45s_3Chunks_GeraTresChunksSemLacuna()
    {
        var details = CreateDetails(durationSec: 45, parallelChunks: 3);
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().HaveCount(3);
        payload.Video.Chunks[0].StartSec.Should().Be(0);
        payload.Video.Chunks[0].EndSec.Should().Be(15);
        payload.Video.Chunks[1].StartSec.Should().Be(15);
        payload.Video.Chunks[1].EndSec.Should().Be(30);
        payload.Video.Chunks[2].StartSec.Should().Be(30);
        payload.Video.Chunks[2].EndSec.Should().Be(45);
    }

    [Fact]
    public void Build_ChunkUnico_45s_1Chunk_EndSec45SemMenosUm()
    {
        var details = CreateDetails(durationSec: 45, parallelChunks: 1);
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().HaveCount(1);
        payload.Video.Chunks[0].StartSec.Should().Be(0);
        payload.Video.Chunks[0].EndSec.Should().Be(45);
        payload.Video.Chunks[0].EndSec.Should().NotBe(-1);
    }

    [Fact]
    public void Build_DivisaoComResto_10s_3Chunks_GeraTresChunksUltimoFechandoEm10()
    {
        var details = CreateDetails(durationSec: 10, parallelChunks: 3);
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().HaveCount(3);
        payload.Video.Chunks[0].StartSec.Should().Be(0);
        payload.Video.Chunks[0].EndSec.Should().Be(4);
        payload.Video.Chunks[1].StartSec.Should().Be(4);
        payload.Video.Chunks[1].EndSec.Should().Be(8);
        payload.Video.Chunks[2].StartSec.Should().Be(8);
        payload.Video.Chunks[2].EndSec.Should().Be(10);
    }

    [Fact]
    public void Build_ParallelChunksInvalidoZero_TratadoComoUmChunk()
    {
        var details = CreateDetails(durationSec: 30, parallelChunks: 0);
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().HaveCount(1);
        payload.Video.Chunks[0].StartSec.Should().Be(0);
        payload.Video.Chunks[0].EndSec.Should().Be(30);
    }

    [Fact]
    public void Build_DurationSecZero_GeraUmChunkFallbackZeroAZero()
    {
        var details = CreateDetails(durationSec: 0, parallelChunks: 3);
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", DefaultOptions());

        payload.Video.Chunks.Should().HaveCount(1);
        payload.Video.Chunks[0].StartSec.Should().Be(0);
        payload.Video.Chunks[0].EndSec.Should().Be(0);
    }

    [Fact]
    public void Build_IntervalSecDoVideo_UsaFrameIntervalSecDoDetails()
    {
        var details = CreateDetails(durationSec: 45, frameIntervalSec: 5, parallelChunks: 1);
        var options = new OutputOptions { FrameIntervalSec = 2 };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        payload.Video.Chunks[0].IntervalSec.Should().Be(5);
    }

    [Fact]
    public void Build_IntervalSecFallback_UsaOutputOptionsQuandoFrameIntervalSecZero()
    {
        var details = CreateDetails(durationSec: 45, frameIntervalSec: 0, parallelChunks: 1);
        var options = new OutputOptions { FrameIntervalSec = 3 };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-1", options);

        payload.Video.Chunks[0].IntervalSec.Should().Be(3);
    }

    [Fact]
    public void Build_Smoke_RestanteDoPayloadInalterado()
    {
        var details = CreateDetails("v2", "u2", durationSec: 60, parallelChunks: 4);
        var options = new OutputOptions
        {
            FramesBucket = "fb",
            ManifestBucket = "mb",
            ZipBucket = "zb",
            OrdenaAutomaticamente = false
        };
        var payload = StepFunctionPayloadBuilder.Build(details, "exec-smoke", options);

        payload.ContractVersion.Should().Be("1.0");
        payload.ExecutionId.Should().Be("exec-smoke");
        payload.Video.VideoId.Should().Be("v2");
        payload.Video.UserId.Should().Be("u2");
        payload.Output.FramesBucket.Should().Be("fb");
        payload.Output.ManifestBucket.Should().Be("mb");
        payload.Zip.OutputBucket.Should().Be("zb");
        payload.Zip.OutputKey.Should().Contain("output.zip");
        payload.Finalize.OrdenaAutomaticamente.Should().BeFalse();
        payload.User.Name.Should().Be("João Silva");
        payload.User.Email.Should().Be("joao@example.com");
        payload.Video.Chunks.Should().HaveCount(4);
    }
}

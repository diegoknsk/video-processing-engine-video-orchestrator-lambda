using System.Text.Json.Serialization;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;

/// <summary>
/// Envelope da API Video Management: { "success", "data", "timestamp" }.
/// </summary>
public sealed class VideoManagementApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public VideoManagementVideoData Data { get; init; } = null!;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; } = "";
}

/// <summary>
/// Payload em "data" do GET /internal/videos/{userId}/{videoId}.
/// </summary>
public sealed class VideoManagementVideoData
{
    [JsonPropertyName("videoId")]
    public string VideoId { get; init; } = "";

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("userEmail")]
    public string UserEmail { get; init; } = "";

    [JsonPropertyName("originalFileName")]
    public string OriginalFileName { get; init; } = "";

    [JsonPropertyName("status")]
    public int Status { get; init; }

    [JsonPropertyName("statusDescription")]
    public string StatusDescription { get; init; } = "";

    [JsonPropertyName("s3BucketVideo")]
    public string S3BucketVideo { get; init; } = "";

    [JsonPropertyName("s3KeyVideo")]
    public string S3KeyVideo { get; init; } = "";

    [JsonPropertyName("durationSec")]
    public int DurationSec { get; init; }

    [JsonPropertyName("frameIntervalSec")]
    public int FrameIntervalSec { get; init; }

    [JsonPropertyName("parallelChunks")]
    public int ParallelChunks { get; init; } = 1;
}

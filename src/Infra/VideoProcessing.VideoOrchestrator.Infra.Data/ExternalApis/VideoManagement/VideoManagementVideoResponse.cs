using System.Text.Json.Serialization;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;

/// <summary>
/// Contrato de resposta da API Video Management (GET /internal/videos/{userId}/{videoId}).
/// </summary>
public sealed class VideoManagementVideoResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("status")]
    public string Status { get; init; } = "";

    [JsonPropertyName("s3Key")]
    public string S3Key { get; init; } = "";

    [JsonPropertyName("user")]
    public VideoManagementUserInfo? User { get; init; }
}

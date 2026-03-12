using Refit;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;

/// <summary>
/// API Refit para Video Management (endpoint interno de detalhes do vídeo).
/// </summary>
public interface IVideoManagementApi
{
    [Get("/internal/videos/{userId}/{videoId}")]
    Task<VideoManagementVideoResponse> GetVideoAsync(
        string userId,
        string videoId,
        [Header("Authorization")] string authorization,
        CancellationToken ct = default);
}

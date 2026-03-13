using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.Ports;

/// <summary>
/// Port para buscar detalhes do vídeo na API Video Management.
/// </summary>
public interface IVideoManagementClient
{
    /// <summary>
    /// Busca detalhes do vídeo por userId e videoId, usando o token de acesso.
    /// </summary>
    Task<VideoDetails> GetVideoDetailsAsync(string userId, string videoId, string accessToken, CancellationToken ct = default);
}

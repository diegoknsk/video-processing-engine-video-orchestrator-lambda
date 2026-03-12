using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.UseCases;

/// <summary>
/// Use case que extrai userId/videoId da key S3, obtém token M2M e busca detalhes do vídeo.
/// </summary>
public interface IFetchVideoDetailsUseCase
{
    /// <summary>
    /// Executa o fluxo: parse da key → token → detalhes do vídeo.
    /// </summary>
    /// <param name="s3Key">Key do objeto S3 (formato: videos/{userId}/{videoId}/original).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Detalhes do vídeo.</returns>
    Task<VideoDetails> ExecuteAsync(string s3Key, CancellationToken ct = default);
}

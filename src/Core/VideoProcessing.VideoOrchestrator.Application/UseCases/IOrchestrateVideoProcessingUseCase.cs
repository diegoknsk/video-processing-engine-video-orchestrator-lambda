using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.UseCases;

/// <summary>
/// Use case que monta o payload da Step Function e dispara a execução.
/// </summary>
public interface IOrchestrateVideoProcessingUseCase
{
    /// <summary>
    /// Monta o payload a partir dos detalhes do vídeo e inicia a execução da Step Function.
    /// </summary>
    /// <param name="videoDetails">Detalhes do vídeo (Video Management).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>O executionArn retornado pela AWS.</returns>
    Task<string> ExecuteAsync(VideoDetails videoDetails, CancellationToken ct = default);
}

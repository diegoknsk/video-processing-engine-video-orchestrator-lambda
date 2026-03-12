using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.Ports;

/// <summary>
/// Porta para disparo da execução da Step Function.
/// </summary>
public interface IStepFunctionService
{
    /// <summary>
    /// Inicia uma execução da Step Function com o payload informado.
    /// </summary>
    /// <param name="payload">Payload completo (serializado como JSON camelCase).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>O executionArn retornado pela AWS.</returns>
    Task<string> StartExecutionAsync(StepFunctionPayload payload, CancellationToken ct = default);
}

namespace VideoProcessing.VideoOrchestrator.Application.Ports;

/// <summary>
/// Port para obter token de acesso M2M (client credentials).
/// </summary>
public interface IM2MTokenService
{
    /// <summary>
    /// Obtém o access token para chamadas M2M.
    /// </summary>
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
}

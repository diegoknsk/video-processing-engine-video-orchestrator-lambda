using Refit;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;

/// <summary>
/// API Refit para autenticação M2M (client credentials).
/// BaseAddress deve ser o TokenEndpoint (URL completa do endpoint de token).
/// </summary>
public interface IM2MAuthApi
{
    [Post("")]
    Task<M2MTokenResponse> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] M2MTokenRequest request, CancellationToken ct = default);
}

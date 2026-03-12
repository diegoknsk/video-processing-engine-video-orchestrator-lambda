using Refit;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;

/// <summary>
/// Request body (form url-encoded) para token OAuth2 client credentials.
/// </summary>
public sealed class M2MTokenRequest
{
    [AliasAs("grant_type")]
    public string GrantType { get; } = "client_credentials";

    [AliasAs("client_id")]
    public string ClientId { get; init; } = "";

    [AliasAs("client_secret")]
    public string ClientSecret { get; init; } = "";
}

using System.Text.Json.Serialization;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;

/// <summary>
/// Response do endpoint de token OAuth2.
/// </summary>
public sealed class M2MTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = "";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}

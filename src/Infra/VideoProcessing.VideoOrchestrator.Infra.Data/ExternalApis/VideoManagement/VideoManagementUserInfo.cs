using System.Text.Json.Serialization;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;

/// <summary>
/// Informações do usuário no contrato da API Video Management.
/// </summary>
public sealed class VideoManagementUserInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("email")]
    public string Email { get; init; } = "";
}

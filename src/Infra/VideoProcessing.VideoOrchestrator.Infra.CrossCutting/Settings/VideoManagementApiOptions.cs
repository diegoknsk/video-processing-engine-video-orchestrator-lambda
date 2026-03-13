using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

/// <summary>
/// Configuração da API interna de Video Management (BaseUrl e Timeout).
/// Binding: seção "VideoManagementApi" (env: VIDEO_MANAGEMENT_API__*).
/// </summary>
public sealed class VideoManagementApiOptions
{
    /// <summary>Nome da seção no IConfiguration (equivale ao prefixo env VIDEO_MANAGEMENT_API__).</summary>
    public const string SectionName = "VIDEO_MANAGEMENT_API";

    [Required]
    [ConfigurationKeyName("BASE_URL")]
    public required string BaseUrl { get; init; }

    [ConfigurationKeyName("TIMEOUT_SECONDS")]
    public int TimeoutSeconds { get; init; } = 30;
}

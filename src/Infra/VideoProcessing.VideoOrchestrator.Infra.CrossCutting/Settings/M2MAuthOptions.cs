using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

/// <summary>
/// Configuração de autenticação M2M (client credentials).
/// Binding: seção "M2MAuth" (env: M2M_AUTH__*).
/// </summary>
public sealed class M2MAuthOptions
{
    /// <summary>Nome da seção no IConfiguration (equivale ao prefixo env M2M_AUTH__).</summary>
    public const string SectionName = "M2M_AUTH";

    [Required]
    [ConfigurationKeyName("TOKEN_ENDPOINT")]
    public required string TokenEndpoint { get; init; }

    [Required]
    [ConfigurationKeyName("CLIENT_ID")]
    public required string ClientId { get; init; }

    [Required]
    [ConfigurationKeyName("CLIENT_SECRET")]
    public required string ClientSecret { get; init; }
}

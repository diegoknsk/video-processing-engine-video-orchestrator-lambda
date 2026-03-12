using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

/// <summary>
/// Configuração da Step Function a ser disparada pelo orquestrador.
/// Binding: seção "StepFunction" (env: STEP_FUNCTION__*).
/// </summary>
public sealed class StepFunctionOptions
{
    /// <summary>Nome da seção no IConfiguration (equivale ao prefixo env STEP_FUNCTION__).</summary>
    public const string SectionName = "STEP_FUNCTION";

    [Required]
    [ConfigurationKeyName("STATE_MACHINE_ARN")]
    public required string StateMachineArn { get; init; }
}

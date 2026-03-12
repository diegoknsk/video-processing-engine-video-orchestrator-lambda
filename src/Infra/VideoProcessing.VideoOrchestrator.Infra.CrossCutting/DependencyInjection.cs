using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

namespace VideoProcessing.VideoOrchestrator.Infra.CrossCutting;

/// <summary>
/// Extensões de DI e configuração tipada do orquestrador.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra configuração tipada (Options) e dependências do orquestrador.
    /// Variáveis de ambiente devem usar o prefixo com duplo underscore (ex.: VIDEO_MANAGEMENT_API__BASE_URL).
    /// </summary>
    public static IServiceCollection AddOrchestratorConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<VideoManagementApiOptions>()
            .Bind(config.GetSection(VideoManagementApiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<M2MAuthOptions>()
            .Bind(config.GetSection(M2MAuthOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<StepFunctionOptions>()
            .Bind(config.GetSection(StepFunctionOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}

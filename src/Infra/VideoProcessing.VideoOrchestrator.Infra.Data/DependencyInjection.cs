using Amazon.Extensions.NETCore.Setup;
using Amazon.StepFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using VideoProcessing.VideoOrchestrator.Infra.Data.AwsServices;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;
using Refit;
using Microsoft.Extensions.Http.Resilience;

namespace VideoProcessing.VideoOrchestrator.Infra.Data;

/// <summary>
/// Registro de dependências de dados (Refit clients, serviços M2M e Video Management).
/// Requer que AddOrchestratorConfiguration tenha sido chamado antes (Options registradas).
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra clientes Refit (M2M Auth, Video Management) e serviços que implementam os Ports.
    /// </summary>
    public static IServiceCollection AddOrchestratorData(this IServiceCollection services, IConfiguration config)
    {
        services.AddRefitClient<IM2MAuthApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<M2MAuthOptions>>().Value;
                client.BaseAddress = new Uri(options.TokenEndpoint);
            })
            .AddStandardResilienceHandler();

        services.AddRefitClient<IVideoManagementApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<VideoManagementApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .AddStandardResilienceHandler();

        services.AddScoped<IM2MTokenService, M2MTokenService>();
        services.AddScoped<IVideoManagementClient, VideoManagementClientService>();

        services.AddAWSService<IAmazonStepFunctions>();
        services.AddScoped<IStepFunctionService, StepFunctionService>();

        return services;
    }
}

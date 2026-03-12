using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Exceptions;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;
using Refit;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;

/// <summary>
/// Implementação do port IM2MTokenService via Refit. Nunca loga ClientSecret ou token.
/// </summary>
public sealed class M2MTokenService(IM2MAuthApi api, IOptions<M2MAuthOptions> options, ILogger<M2MTokenService> logger) : IM2MTokenService
{
    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        var opts = options.Value;
        var request = new M2MTokenRequest { ClientId = opts.ClientId, ClientSecret = opts.ClientSecret };

        try
        {
            logger.LogDebug("Requesting M2M access token");
            var response = await api.GetTokenAsync(request, ct);
            return response.AccessToken;
        }
        catch (ApiException ex)
        {
            throw new ExternalServiceException("M2M token request failed.", ex);
        }
    }
}

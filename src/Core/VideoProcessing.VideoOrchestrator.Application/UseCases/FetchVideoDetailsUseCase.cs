using Microsoft.Extensions.Logging;
using VideoProcessing.VideoOrchestrator.Application.Parsers;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.UseCases;

/// <summary>
/// Orquestra: S3KeyParser.Parse → token M2M → VideoManagementClient.GetVideoDetailsAsync.
/// </summary>
public sealed class FetchVideoDetailsUseCase(
    IM2MTokenService tokenService,
    IVideoManagementClient videoManagementClient,
    ILogger<FetchVideoDetailsUseCase> logger) : IFetchVideoDetailsUseCase
{
    public async Task<VideoDetails> ExecuteAsync(string s3Key, CancellationToken ct = default)
    {
        var (userId, videoId) = S3KeyParser.Parse(s3Key);

        logger.LogInformation("Fetching video details for UserId {UserId}, VideoId {VideoId}", userId, videoId);

        var accessToken = await tokenService.GetAccessTokenAsync(ct);
        var details = await videoManagementClient.GetVideoDetailsAsync(userId, videoId, accessToken, ct);

        logger.LogInformation("Video details retrieved for UserId {UserId}, VideoId {VideoId}", userId, videoId);

        return details;
    }
}

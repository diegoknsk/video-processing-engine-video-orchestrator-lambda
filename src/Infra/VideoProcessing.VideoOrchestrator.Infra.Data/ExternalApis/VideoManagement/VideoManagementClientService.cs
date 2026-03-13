using System.Net;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Exceptions;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;
using Refit;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.VideoManagement;

/// <summary>
/// Implementação do port IVideoManagementClient. Token enviado como "Bearer {token}".
/// 404 → NotFoundException; outros 4xx/5xx → ExternalServiceException.
/// </summary>
public sealed class VideoManagementClientService(IVideoManagementApi api) : IVideoManagementClient
{
    public async Task<VideoDetails> GetVideoDetailsAsync(string userId, string videoId, string accessToken, CancellationToken ct = default)
    {
        var authorization = "Bearer " + accessToken;

        try
        {
            var response = await api.GetVideoAsync(userId, videoId, authorization, ct);
            var d = response.Data;
            return new VideoDetails(
                VideoId: d.VideoId,
                UserId: d.UserId,
                Title: d.OriginalFileName,
                Status: d.StatusDescription,
                S3Key: d.S3KeyVideo,
                S3Bucket: d.S3BucketVideo,
                UserName: "",
                UserEmail: d.UserEmail,
                DurationSec: d.DurationSec,
                FrameIntervalSec: d.FrameIntervalSec,
                ParallelChunks: d.ParallelChunks > 0 ? d.ParallelChunks : 1
            );
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException($"Video not found: userId={userId}, videoId={videoId}.", ex);

            throw new ExternalServiceException($"Video Management API error: {ex.StatusCode}.", ex);
        }
    }
}

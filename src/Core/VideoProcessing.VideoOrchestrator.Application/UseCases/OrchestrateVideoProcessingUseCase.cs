using Microsoft.Extensions.Logging;
using VideoProcessing.VideoOrchestrator.Application.Builders;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.UseCases;

/// <summary>
/// Monta o payload via StepFunctionPayloadBuilder e dispara a Step Function via IStepFunctionService.
/// </summary>
public sealed class OrchestrateVideoProcessingUseCase(
    IStepFunctionService stepFunctionService,
    ILogger<OrchestrateVideoProcessingUseCase> logger) : IOrchestrateVideoProcessingUseCase
{
    public async Task<string> ExecuteAsync(VideoDetails videoDetails, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Building Step Function payload for VideoId={VideoId}, UserId={UserId}",
            videoDetails.VideoId, videoDetails.UserId);

        var executionId = $"exec-{videoDetails.VideoId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var payload = StepFunctionPayloadBuilder.Build(videoDetails, executionId);

        logger.LogInformation(
            "Dispatching Step Function for VideoId={VideoId}, ExecutionId={ExecutionId}",
            videoDetails.VideoId, executionId);

        var executionArn = await stepFunctionService.StartExecutionAsync(payload, ct);

        logger.LogInformation(
            "Step Function dispatched successfully. VideoId={VideoId}, ExecutionArn={ExecutionArn}",
            videoDetails.VideoId, executionArn);

        return executionArn;
    }
}

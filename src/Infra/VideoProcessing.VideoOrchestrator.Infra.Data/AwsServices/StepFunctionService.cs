using System.Text.Json;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

namespace VideoProcessing.VideoOrchestrator.Infra.Data.AwsServices;

/// <summary>
/// Dispara a execução da Step Function via AWS SDK; ARN e região vêm de configuração.
/// </summary>
public sealed class StepFunctionService(
    IAmazonStepFunctions stepFunctions,
    IOptions<StepFunctionOptions> options,
    ILogger<StepFunctionService> logger) : IStepFunctionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<string> StartExecutionAsync(StepFunctionPayload payload, CancellationToken ct = default)
    {
        var stateMachineArn = options.Value.StateMachineArn;
        var inputJson = JsonSerializer.Serialize(payload, JsonOptions);

        logger.LogInformation(
            "Starting Step Function execution for VideoId={VideoId}, ExecutionId={ExecutionId}",
            payload.Video.VideoId, payload.ExecutionId);

        var request = new StartExecutionRequest
        {
            StateMachineArn = stateMachineArn,
            Name = payload.ExecutionId,
            Input = inputJson
        };

        try
        {
            var response = await stepFunctions.StartExecutionAsync(request, ct);
            logger.LogInformation(
                "Step Function execution started: ExecutionArn={ExecutionArn}",
                response.ExecutionArn);
            return response.ExecutionArn;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to start Step Function execution for VideoId={VideoId}",
                payload.Video.VideoId);
            throw;
        }
    }
}

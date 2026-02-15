using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using VideoOrchestrator.Models;

namespace VideoOrchestrator.Handler;

/// <summary>
/// Handler puro para AWS Lambda, trigger SQS.
/// Entry point: VideoOrchestrator::VideoOrchestrator.Handler.VideoOrchestratorHandler::HandleAsync
/// </summary>
public class VideoOrchestratorHandler
{
    public async Task<SQSBatchResponse> HandleAsync(SQSEvent sqsEvent, ILambdaContext context)
    {
        var requestId = context.AwsRequestId;
        var correlationId = requestId; // Usa requestId como correlationId para rastreamento

        var mockResponse = new MockResponse("ok", requestId, correlationId);
        var mockJson = JsonSerializer.Serialize(mockResponse);
        context.Logger.LogInformation("MOCK Response: {MockResponse}", mockJson);

        context.Logger.LogInformation("Início do processamento. RequestId={RequestId}, CorrelationId={CorrelationId}",
            requestId, correlationId);

        var batchResponse = new SQSBatchResponse();

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                await ProcessMessageAsync(record, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex, "Erro ao processar mensagem MessageId={MessageId}", record.MessageId);
                batchResponse.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure
                {
                    ItemIdentifier = record.MessageId
                });
            }
        }

        context.Logger.LogInformation("Fim do processamento. RequestId={RequestId}, CorrelationId={CorrelationId}, Processados={Count}",
            requestId, correlationId, sqsEvent.Records.Count - batchResponse.BatchItemFailures.Count);

        return batchResponse;
    }

    private static Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        var requestId = context.AwsRequestId;
        context.Logger.LogInformation("Mensagem recebida. MessageId={MessageId}, RequestId={RequestId}",
            message.MessageId, requestId);

        return Task.CompletedTask;
    }
}

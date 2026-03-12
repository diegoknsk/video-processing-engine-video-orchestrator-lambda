using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Application.UseCases;
using VideoProcessing.VideoOrchestrator.Domain.Events;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using VideoProcessing.VideoOrchestrator.Infra.Data;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VideoProcessing.VideoOrchestrator.Lambda
{
    public class Function
    {
        private readonly IServiceProvider _serviceProvider;

        public Function()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            });

            services.AddOrchestratorConfiguration(config);
            services.AddOrchestratorData(config);

            _serviceProvider = services.BuildServiceProvider(validateScopes: true);

            // Dispara validação das Options no startup (ValidateOnStart)
            _ = _serviceProvider.GetRequiredService<IOptions<VideoManagementApiOptions>>().Value;
            _ = _serviceProvider.GetRequiredService<IOptions<M2MAuthOptions>>().Value;
            _ = _serviceProvider.GetRequiredService<IOptions<StepFunctionOptions>>().Value;
        }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                context.Logger.LogInformation("Processing record {MessageId}", record.MessageId);

                S3ObjectCreatedEvent? s3Event;
                try
                {
                    s3Event = JsonSerializer.Deserialize<S3ObjectCreatedEvent>(record.Body);
                }
                catch (JsonException ex)
                {
                    context.Logger.LogError(ex, "Falha ao deserializar body S3 do record {MessageId}", record.MessageId);
                    throw;
                }

                if (s3Event?.Records is not { Count: > 0 })
                {
                    context.Logger.LogWarning("Record {MessageId} sem Records S3 válidas, ignorado", record.MessageId);
                    continue;
                }

                using var scope = _serviceProvider.CreateScope();
                var fetchUseCase = scope.ServiceProvider.GetRequiredService<IFetchVideoDetailsUseCase>();
                var orchestrateUseCase = scope.ServiceProvider.GetRequiredService<IOrchestrateVideoProcessingUseCase>();

                foreach (var s3Record in s3Event.Records)
                {
                    var bucketName = s3Record.S3?.Bucket?.Name ?? "";
                    var s3Key = s3Record.S3?.Object?.Key;
                    if (string.IsNullOrEmpty(s3Key))
                    {
                        context.Logger.LogWarning("Record {MessageId}: S3 object key ausente, bucket={Bucket}", record.MessageId, bucketName);
                        continue;
                    }

                    context.Logger.LogInformation("Processando S3 bucket={Bucket}, key={Key}", bucketName, s3Key);

                    var videoDetails = await fetchUseCase.ExecuteAsync(s3Key);
                    var detailsWithBucket = videoDetails with { S3Bucket = bucketName };
                    var executionArn = await orchestrateUseCase.ExecuteAsync(detailsWithBucket);

                    context.Logger.LogInformation(
                        "Orchestration complete for VideoId={VideoId}. ExecutionArn={ExecutionArn}",
                        videoDetails.VideoId, executionArn);
                }
            }
        }
    }
}

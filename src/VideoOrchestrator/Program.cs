using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using VideoOrchestrator.Handler;

var handler = new VideoOrchestratorHandler();
await LambdaBootstrapBuilder.Create<SQSEvent, SQSBatchResponse>(handler.HandleAsync, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();

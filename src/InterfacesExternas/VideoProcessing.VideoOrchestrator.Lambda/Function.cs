using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;

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
                context.Logger.LogInformation("Record recebido: {MessageId}", record.MessageId);
            }

            await Task.CompletedTask;
        }
    }
}

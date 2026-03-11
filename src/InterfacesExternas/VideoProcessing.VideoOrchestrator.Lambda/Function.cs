using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VideoProcessing.VideoOrchestrator.Lambda
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input">The event for the Lambda function handler to process.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var message in sqsEvent.Records)
            {
                var body = message.Body;

               Console.WriteLine($"Mensagem recebida: {body}");

                // aqui você desserializa
            }

        }
    }
}

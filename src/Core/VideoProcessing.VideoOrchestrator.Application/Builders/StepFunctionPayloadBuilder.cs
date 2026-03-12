using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.Builders;

/// <summary>
/// Monta o payload da Step Function a partir de VideoDetails.
/// </summary>
public static class StepFunctionPayloadBuilder
{
    /// <summary>
    /// Constrói o payload com outputPrefix, chunks (ao menos um), zip e user.
    /// </summary>
    public static StepFunctionPayload Build(VideoDetails details, string executionId)
    {
        var outputPrefix = $"videos/{details.UserId}/{details.VideoId}/frames/";

        var chunks = new List<VideoChunk>
        {
            new(ChunkIndex: 0, OutputPath: $"{outputPrefix}chunk-0/")
        };

        return new StepFunctionPayload(
            ExecutionId: executionId,
            Video: new VideoProcessingInput(
                VideoId: details.VideoId,
                UserId: details.UserId,
                Title: details.Title,
                S3Bucket: details.S3Bucket,
                S3Key: details.S3Key,
                OutputPrefix: outputPrefix,
                Chunks: chunks
            ),
            Zip: new ZipOutputInfo(
                OutputBucket: details.S3Bucket,
                OutputKey: $"videos/{details.UserId}/{details.VideoId}/output.zip"
            ),
            User: new UserInfo(
                Name: details.UserName,
                Email: details.UserEmail
            )
        );
    }
}

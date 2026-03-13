using VideoProcessing.VideoOrchestrator.Application.Options;
using VideoProcessing.VideoOrchestrator.Domain.Models;

namespace VideoProcessing.VideoOrchestrator.Application.Builders;

/// <summary>
/// Monta o payload da Step Function a partir de VideoDetails e OutputOptions.
/// Contrato: contractVersion, executionId, video (chunks com tempo e prefixos), output, zip, finalize, user.
/// </summary>
public static class StepFunctionPayloadBuilder
{
    private const string ContractVersionValue = "1.0";

    /// <summary>
    /// Constrói o payload com contractVersion, output, finalize e chunks enriquecidos (chunkId, startSec, endSec, intervalSec, manifestPrefix, framesPrefix).
    /// Todos os buckets vêm de outputOptions — nenhum hardcode.
    /// </summary>
    public static StepFunctionPayload Build(VideoDetails details, string executionId, OutputOptions outputOptions)
    {
        var intervalSec = details.FrameIntervalSec > 0 ? details.FrameIntervalSec : outputOptions.FrameIntervalSec;
        var chunks = BuildChunks(details, intervalSec);

        var outputPrefix = $"videos/{details.UserId}/{details.VideoId}/frames/";
        var framesBasePrefix = $"videos/{details.UserId}/{details.VideoId}/frames/";

        return new StepFunctionPayload(
            ContractVersion: ContractVersionValue,
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
            Output: new OutputInfo(
                FramesBucket: outputOptions.FramesBucket,
                ManifestBucket: outputOptions.ManifestBucket
            ),
            Zip: new ZipOutputInfo(
                OutputBucket: outputOptions.ZipBucket,
                OutputKey: $"videos/{details.UserId}/{details.VideoId}/output.zip"
            ),
            Finalize: new FinalizeInfo(
                FramesBucket: outputOptions.FramesBucket,
                FramesBasePrefix: framesBasePrefix,
                OutputBucket: outputOptions.ZipBucket,
                VideoId: details.VideoId,
                OutputBasePrefix: $"{details.UserId}/{details.VideoId}",
                OrdenaAutomaticamente: outputOptions.OrdenaAutomaticamente
            ),
            User: new UserInfo(
                Name: details.UserName,
                Email: details.UserEmail
            )
        );
    }

    private static List<VideoChunk> BuildChunks(VideoDetails details, int intervalSec)
    {
        var parallelChunks = details.ParallelChunks > 0 ? details.ParallelChunks : 1;
        var duration = details.DurationSec;

        if (duration <= 0)
            return
            [
                new VideoChunk(
                    ChunkIndex: 0,
                    ChunkId: $"{details.VideoId}-chunk-0",
                    StartSec: 0,
                    EndSec: 0,
                    IntervalSec: intervalSec,
                    ManifestPrefix: $"videos/{details.UserId}/{details.VideoId}/manifests/chunk-0/",
                    FramesPrefix: $"videos/{details.UserId}/{details.VideoId}/frames/chunk-0/"
                )
            ];

        var chunkDuration = (int)Math.Ceiling((double)duration / parallelChunks);
        var chunks = new List<VideoChunk>(parallelChunks);

        for (var i = 0; i < parallelChunks; i++)
        {
            var startSec = i * chunkDuration;
            var endSec = Math.Min((i + 1) * chunkDuration, duration);
            chunks.Add(new VideoChunk(
                ChunkIndex: i,
                ChunkId: $"{details.VideoId}-chunk-{i}",
                StartSec: startSec,
                EndSec: endSec,
                IntervalSec: intervalSec,
                ManifestPrefix: $"videos/{details.UserId}/{details.VideoId}/manifests/chunk-{i}/",
                FramesPrefix: $"videos/{details.UserId}/{details.VideoId}/frames/chunk-{i}/"
            ));
        }

        return chunks;
    }
}

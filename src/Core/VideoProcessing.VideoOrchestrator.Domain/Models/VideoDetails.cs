namespace VideoProcessing.VideoOrchestrator.Domain.Models;

/// <summary>
/// Detalhes do vídeo retornados pela API Video Management (dados para Story 04).
/// </summary>
public sealed record VideoDetails(
    string VideoId,
    string UserId,
    string Title,
    string Status,
    string S3Key,
    string S3Bucket,
    string UserName,
    string UserEmail,
    int DurationSec,
    int FrameIntervalSec,
    int ParallelChunks
);

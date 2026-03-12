namespace VideoProcessing.VideoOrchestrator.Domain.Models;

/// <summary>
/// Payload raiz enviado como input da Step Function (serializado como JSON camelCase).
/// </summary>
public sealed record StepFunctionPayload(
    string ExecutionId,
    VideoProcessingInput Video,
    ZipOutputInfo Zip,
    UserInfo User
);

/// <summary>
/// Dados do vídeo no payload da Step Function.
/// </summary>
public sealed record VideoProcessingInput(
    string VideoId,
    string UserId,
    string Title,
    string S3Bucket,
    string S3Key,
    string OutputPrefix,
    List<VideoChunk> Chunks
);

/// <summary>
/// Segmento de processamento (chunk) para Map State.
/// </summary>
public sealed record VideoChunk(
    int ChunkIndex,
    string OutputPath
);

/// <summary>
/// Destino do arquivo zip de saída.
/// </summary>
public sealed record ZipOutputInfo(
    string OutputBucket,
    string OutputKey
);

/// <summary>
/// Dados do usuário (Video Management), sem consulta ao Cognito.
/// </summary>
public sealed record UserInfo(
    string Name,
    string Email
);

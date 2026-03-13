namespace VideoProcessing.VideoOrchestrator.Domain.Models;

/// <summary>
/// Payload raiz enviado como input da Step Function (serializado como JSON camelCase).
/// Inclui contractVersion, output e finalize para o novo contrato.
/// </summary>
public sealed record StepFunctionPayload(
    string ContractVersion,
    string ExecutionId,
    VideoProcessingInput Video,
    OutputInfo Output,
    ZipOutputInfo Zip,
    FinalizeInfo Finalize,
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
/// Inclui identificador, intervalo de tempo e prefixos de manifest/frames.
/// </summary>
public sealed record VideoChunk(
    int ChunkIndex,
    string ChunkId,
    int StartSec,
    int EndSec,
    int IntervalSec,
    string ManifestPrefix,
    string FramesPrefix
);

/// <summary>
/// Buckets de saída para frames e manifestos (configuráveis via OutputOptions).
/// </summary>
public sealed record OutputInfo(
    string FramesBucket,
    string ManifestBucket
);

/// <summary>
/// Destino do arquivo zip de saída.
/// </summary>
public sealed record ZipOutputInfo(
    string OutputBucket,
    string OutputKey
);

/// <summary>
/// Dados para a etapa de finalização (redundância intencional para desacoplar o lambda de finalize).
/// </summary>
public sealed record FinalizeInfo(
    string FramesBucket,
    string FramesBasePrefix,
    string OutputBucket,
    string VideoId,
    string OutputBasePrefix,
    bool OrdenaAutomaticamente
);

/// <summary>
/// Dados do usuário (Video Management), sem consulta ao Cognito.
/// </summary>
public sealed record UserInfo(
    string Name,
    string Email
);

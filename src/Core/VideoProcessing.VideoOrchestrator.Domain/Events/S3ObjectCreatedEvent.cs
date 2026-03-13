using System.Text.Json.Serialization;

namespace VideoProcessing.VideoOrchestrator.Domain.Events;

/// <summary>
/// Representa o evento S3 Object Created (notificação S3), sem dependência do SDK Lambda.
/// Usado para deserializar o body de mensagens SQS que contêm notificações S3.
/// </summary>
public sealed record S3ObjectCreatedEvent(
    [property: JsonPropertyName("Records")] IReadOnlyList<S3Record>? Records
);

/// <summary>
/// Um record do evento S3 (cada item em Records).
/// </summary>
public sealed record S3Record(
    [property: JsonPropertyName("s3")] S3Detail? S3
);

/// <summary>
/// Detalhe S3 contendo bucket e object.
/// </summary>
public sealed record S3Detail(
    [property: JsonPropertyName("bucket")] S3Bucket? Bucket,
    [property: JsonPropertyName("object")] S3Object? Object
);

/// <summary>
/// Informações do bucket S3.
/// </summary>
public sealed record S3Bucket(
    [property: JsonPropertyName("name")] string? Name
);

/// <summary>
/// Informações do objeto S3 (inclui a key).
/// </summary>
public sealed record S3Object(
    [property: JsonPropertyName("key")] string? Key
);

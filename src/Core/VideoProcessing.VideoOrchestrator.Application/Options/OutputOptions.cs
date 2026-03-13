namespace VideoProcessing.VideoOrchestrator.Application.Options;

/// <summary>
/// Configuração dos buckets de saída e parâmetros de processamento.
/// Binding: seção "OUTPUT" (env: OUTPUT__*). Nenhuma propriedade é obrigatória; todas têm default.
/// </summary>
public sealed class OutputOptions
{
    /// <summary>Nome da seção no IConfiguration (equivale ao prefixo env OUTPUT__).</summary>
    public const string SectionName = "OUTPUT";

    /// <summary>Bucket onde os frames extraídos são gravados.</summary>
    public string FramesBucket { get; init; } = "video-processing-engine-dev-images";

    /// <summary>Bucket onde os manifestos de chunk são gravados.</summary>
    public string ManifestBucket { get; init; } = "video-processing-engine-dev-images";

    /// <summary>Bucket onde o zip de saída é gravado.</summary>
    public string ZipBucket { get; init; } = "video-processing-engine-dev-zip";

    /// <summary>Intervalo em segundos entre frames extraídos por chunk.</summary>
    public int FrameIntervalSec { get; init; } = 1;

    /// <summary>Se true, a etapa de finalização ordena automaticamente os frames.</summary>
    public bool OrdenaAutomaticamente { get; init; } = true;
}

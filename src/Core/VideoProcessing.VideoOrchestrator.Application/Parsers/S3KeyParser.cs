namespace VideoProcessing.VideoOrchestrator.Application.Parsers;

/// <summary>
/// Parser estático para extrair userId e videoId da key S3.
/// Formato esperado: videos/{userId}/{videoId}/original
/// </summary>
public static class S3KeyParser
{
    private const string ExpectedPrefix = "videos/";
    private const string ExpectedSuffix = "/original";

    /// <summary>
    /// Extrai UserId e VideoId da key S3. Formato: videos/{userId}/{videoId}/original.
    /// </summary>
    /// <param name="key">Key do objeto S3.</param>
    /// <returns>Tupla (UserId, VideoId).</returns>
    /// <exception cref="ArgumentException">Key null ou vazia.</exception>
    /// <exception cref="FormatException">Formato da key inválido (prefixo, sufixo ou segmentos).</exception>
    public static (string UserId, string VideoId) Parse(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("S3 key cannot be null or empty.", nameof(key));

        if (!key.StartsWith(ExpectedPrefix, StringComparison.Ordinal))
            throw new FormatException($"S3 key must start with '{ExpectedPrefix}'.");

        if (!key.EndsWith(ExpectedSuffix, StringComparison.Ordinal))
            throw new FormatException($"S3 key must end with '{ExpectedSuffix}'.");

        var withoutPrefix = key[ExpectedPrefix.Length..];
        var withoutSuffix = withoutPrefix[..^ExpectedSuffix.Length];
        var segments = withoutSuffix.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 2)
            throw new FormatException($"S3 key must have exactly two path segments between prefix and suffix (userId/videoId). Got {segments.Length}.");

        return (segments[0], segments[1]);
    }
}

namespace VideoProcessing.VideoOrchestrator.Domain.Exceptions;

/// <summary>
/// Exceção de domínio quando um recurso não é encontrado (ex.: 404 na API externa).
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}

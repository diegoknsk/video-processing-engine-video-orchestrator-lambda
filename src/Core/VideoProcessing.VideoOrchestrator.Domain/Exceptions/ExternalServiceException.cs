namespace VideoProcessing.VideoOrchestrator.Domain.Exceptions;

/// <summary>
/// Exceção de domínio para falhas em serviços externos (APIs, auth, etc.).
/// </summary>
public sealed class ExternalServiceException : Exception
{
    public ExternalServiceException(string message) : base(message) { }

    public ExternalServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}

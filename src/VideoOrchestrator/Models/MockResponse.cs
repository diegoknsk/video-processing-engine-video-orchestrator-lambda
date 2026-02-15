namespace VideoOrchestrator.Models;

/// <summary>
/// Resposta MOCK para validação de execução do Lambda.
/// Utilizada em logs para comprovar que o handler está executando (status, requestId, correlationId).
/// </summary>
public record MockResponse(string Status, string RequestId, string CorrelationId);

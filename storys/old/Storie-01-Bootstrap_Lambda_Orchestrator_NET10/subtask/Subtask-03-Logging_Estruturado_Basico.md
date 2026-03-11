# Subtask 03: Logging estruturado básico

## Descrição
Adicionar logging estruturado básico ao handler (ex.: requestId do Lambda context, correlationId se disponível, mensagem de início e fim de processamento) usando o padrão suportado pelo Lambda .NET (ILambdaContext, Logger do Lambda ou ILogger quando aplicável).

## Passos de implementação
1. Obter no handler o `ILambdaContext` (ou equivalente) e extrair `RequestId` (e `AwsRequestId`).
2. Registrar ao menos um log de início e um de fim de processamento com requestId (e correlationId se definido).
3. Usar formato estruturado quando possível (ex.: JSON ou pares chave-valor) para facilitar consulta no CloudWatch Logs.

## Formas de teste
1. Revisão de código: presença de logs com requestId e mensagens claras.
2. Execução local com mock de contexto (se houver ferramenta de teste local) e verificação de saída de log.
3. Após deploy (Storie-02): validar nos CloudWatch Logs que os campos aparecem.

## Critérios de aceite da subtask
- [ ] Logs de início e fim de processamento presentes no handler.
- [ ] RequestId (e/ou AwsRequestId) incluído nos logs.
- [ ] Formato de log adequado para CloudWatch (estruturado ou legível).

# Storie-06: Tratamento de erro, retry e DLQ

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como Orchestrator do Video Processing Engine, quero tratar erros de forma que falhas transitórias permitam retry pela SQS e falhas permanentes sejam enviadas à DLQ, para garantir confiabilidade e rastreabilidade sem perda de mensagens.

## Objetivo
Definir e implementar a estratégia de tratamento de erros: classificar erros em transitórios (throttling, timeout, 5xx) vs permanentes (payload inválido, validação, 4xx de negócio); em caso de erro transitório, deixar a mensagem falhar (não deletar) para retry automático da SQS; em caso de erro permanente, permitir redrive para DLQ. Resposta do handler (SQSBatchResponse) deve refletir quais mensagens foram processadas com sucesso e quais falharam.

## Escopo Técnico
- Tecnologias: .NET 10, AWS SQS (batch response, visibility timeout, redrive policy), padrões de retry
- Arquivos afetados: handler (tratamento de exceções, construção de SQSBatchResponse), classificador de erros
- Componentes: mapeamento exceção → transitório/permanente, decisão de deletar ou não cada message do batch, logs de falha
- Pacotes/Dependências: Amazon.Lambda.SQSEvents (BatchItemFailure), SDK AWS (exceções conhecidas)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-03 a Storie-05 (fluxo completo de parsing, idempotência e Step Functions).
- Riscos: configuração da fila SQS (visibility timeout, maxReceiveCount, DLQ) deve estar alinhada; pré-condição: DLQ configurada na fila de origem.

## Subtasks
- [Subtask 01: Classificação de erros (transitório vs permanente)](./subtask/Subtask-01-Classificacao_Erros_Transitorio_Permanente.md)
- [Subtask 02: Resposta em batch (SQSBatchResponse e BatchItemFailure)](./subtask/Subtask-02-SQSBatchResponse_BatchItemFailure.md)
- [Subtask 03: Estratégia de DLQ e documentação](./subtask/Subtask-03-Estrategia_DLQ_Documentacao.md)

## Critérios de Aceite da História
- [ ] Erros transitórios (ex.: ThrottlingException, timeout, ServiceUnavailable) não resultam em remoção da mensagem; mensagem volta para a fila (retry)
- [ ] Erros permanentes (payload inválido, validação falha, ConditionalCheckFailed já tratado como sucesso) permitem remoção da mensagem ou redrive para DLQ conforme configuração SQS
- [ ] Handler retorna SQSBatchResponse com batchItemFailures apenas para mensagens que devem ser reprocessadas (transitórios); mensagens processadas com sucesso são removidas
- [ ] Estratégia de DLQ documentada (maxReceiveCount, redrive policy, tratamento de mensagens na DLQ)
- [ ] Logs de falha incluem correlationId e motivo (transitório/permanente) para observabilidade

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

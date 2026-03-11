# Subtask 02: Resposta em batch (SQSBatchResponse e BatchItemFailure)

## Descrição
Implementar a construção da resposta do handler (SQSBatchResponse) de forma que mensagens processadas com sucesso sejam removidas da fila e mensagens que falharam por erro transitório apareçam em batchItemFailures para retry.

## Passos de implementação
1. Para cada mensagem do batch SQS: em sucesso (ou idempotência "já processado"), não incluir na lista de batchItemFailures; em falha transitória, incluir o messageId em BatchItemFailures.
2. Em falha permanente: definir se a mensagem é reportada como failure (para após maxReceiveCount ir para DLQ) ou removida (não retry); documentar decisão.
3. Garantir que partial batch failure seja tratado: apenas as mensagens em batchItemFailures voltam à fila (comportamento padrão Lambda + SQS).

## Formas de teste
1. Teste unitário: batch com 3 mensagens, 2 sucesso e 1 transitório → resposta com 1 batchItemFailure.
2. Teste unitário: batch com 1 mensagem permanente (ex.: payload inválido) → comportamento definido (0 ou 1 failure conforme política).
3. Revisão: documentação AWS Lambda SQS batch e partial batch failure respeitada.

## Critérios de aceite da subtask
- [ ] SQSBatchResponse construído corretamente com BatchItemFailures.
- [ ] Mensagens com sucesso não aparecem em batchItemFailures (são removidas da fila).
- [ ] Mensagens com falha transitória aparecem em batchItemFailures (retry).

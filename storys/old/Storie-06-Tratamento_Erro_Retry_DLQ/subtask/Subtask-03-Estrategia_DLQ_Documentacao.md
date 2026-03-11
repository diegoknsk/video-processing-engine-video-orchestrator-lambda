# Subtask 03: Estratégia de DLQ e documentação

## Descrição
Documentar a estratégia de DLQ (configuração da fila SQS: redrive policy, maxReceiveCount, tratamento de mensagens na DLQ) e garantir que o código esteja alinhado (falhas permanentes levam à DLQ após N retentativas).

## Passos de implementação
1. Documentar configuração recomendada: maxReceiveCount (ex.: 3 ou 5), redrive policy para a DLQ, visibility timeout da fila de origem.
2. Documentar o que fazer com mensagens na DLQ (alertas, inspeção, reprocessamento manual ou não).
3. Garantir que o comportamento do handler (quem vai para batchItemFailures) esteja alinhado: após maxReceiveCount retries, mensagens com falha transitória vão para DLQ; mensagens com falha permanente podem ir na primeira vez ou após 1 retry conforme política.

## Formas de teste
1. Revisão da documentação: outro desenvolvedor consegue configurar a fila e a DLQ.
2. Teste de integração (opcional): enviar mensagem inválida e verificar que após maxReceiveCount ela aparece na DLQ.
3. Checklist de infra: redrive policy, dead-letter queue ARN, permissões.

## Critérios de aceite da subtask
- [ ] Estratégia de DLQ documentada (maxReceiveCount, redrive policy, uso da DLQ).
- [ ] Código alinhado à estratégia (quem falha e quantas vezes antes de DLQ).
- [ ] Documentação inclui variáveis de ambiente ou configuração de infra relacionada (quando aplicável).

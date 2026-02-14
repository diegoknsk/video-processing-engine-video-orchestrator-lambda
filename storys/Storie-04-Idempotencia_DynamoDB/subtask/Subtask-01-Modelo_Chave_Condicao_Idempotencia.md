# Subtask 01: Definição do modelo de chave e condição de idempotência

## Descrição
Definir o modelo de chave (pk, sk) e a condição de idempotência para o DynamoDB: em qual atributo e com qual condição (ex.: attribute_not_exists(stepExecutionArn) ou status = "PENDING") o update será permitido, garantindo alinhamento com o modelo Video Management.

## Passos de implementação
1. Documentar uso da tabela (mesma do Video Management ou tabela de controle): pk = USER#userId, sk = VIDEO#videoId.
2. Definir a ConditionExpression: ex. "attribute_not_exists(stepExecutionArn)" ou "status = :pending", e o que será atualizado na primeira escrita (status, timestamp, etc.).
3. Documentar o comportamento esperado: sucesso na primeira vez; condição falha na segunda (e como o handler interpreta isso como idempotência ok).

## Formas de teste
1. Revisão de documento: condição e chave claras e alinhadas ao Video Management.
2. Teste unitário ou script: simular UpdateItem com condição e verificar resposta (ConditionalCheckFailedException quando já processado).
3. Validar com time/arquitetura que a condição evita duplicidade de execução.

## Critérios de aceite da subtask
- [ ] Modelo de chave (pk/sk) documentado e alinhado ao Video Management.
- [ ] ConditionExpression de idempotência definida e documentada.
- [ ] Comportamento de primeira vs. reprocessamento documentado.

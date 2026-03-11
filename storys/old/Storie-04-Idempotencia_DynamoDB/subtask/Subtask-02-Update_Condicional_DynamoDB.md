# Subtask 02: Implementar update condicional (ConditionExpression)

## Descrição
Implementar a operação de update condicional no DynamoDB (UpdateItem com ConditionExpression), incluindo tratamento da resposta quando a condição falha (ConditionalCheckFailedException) para identificar reprocessamento idempotente.

## Passos de implementação
1. Implementar cliente/repositório que chama UpdateItem com pk, sk, UpdateExpression (set status, updatedAt, etc.) e ConditionExpression definida na Subtask 01.
2. Tratar ConditionalCheckFailedException: retornar resultado tipado indicando "já processado" (idempotência ok).
3. Tratar outras exceções (throttling, acesso) para permitir retry ou falha conforme estratégia de erro (Storie-06).

## Formas de teste
1. Teste unitário com mock do DynamoDB: condição atendida → sucesso; ConditionalCheckFailedException → resultado "já processado".
2. Teste de integração (opcional) contra tabela real ou LocalStack: primeira chamada atualiza; segunda falha na condição.
3. Revisão: exceções não tratadas não vazam; retorno permite ao handler decidir (prosseguir ou retornar sucesso idempotente).

## Critérios de aceite da subtask
- [ ] UpdateItem com ConditionExpression implementado.
- [ ] ConditionalCheckFailedException tratada e mapeada para "idempotência ok".
- [ ] Outras exceções tratadas de forma a permitir retry ou DLQ conforme estratégia.

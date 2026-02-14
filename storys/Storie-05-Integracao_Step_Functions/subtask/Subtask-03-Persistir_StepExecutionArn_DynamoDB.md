# Subtask 03: Persistir stepExecutionArn e status no DynamoDB

## Descrição
Após StartExecution bem-sucedido, atualizar o item do vídeo no DynamoDB com stepExecutionArn e status (ex.: PROCESSING), usando pk/sk (USER#userId, VIDEO#videoId).

## Passos de implementação
1. Implementar update no DynamoDB (UpdateItem) para set stepExecutionArn e status no item já "reservado" pela idempotência (Storie-04).
2. Garantir que o update use a mesma chave (pk, sk) e que não sobrescreva dados críticos do Video Management.
3. Em caso de falha no update (ex.: throttling), definir política: log + retry ou considerar execução já iniciada e apenas log (consistência eventual).

## Formas de teste
1. Teste unitário: após StartExecution retornar ARN, update é chamado com stepExecutionArn e status corretos.
2. Teste de integração (opcional): fluxo completo e verificação no DynamoDB do item atualizado.
3. Revisão: persistência correta do stepExecutionArn documentada e implementada.

## Critérios de aceite da subtask
- [ ] stepExecutionArn persistido no item do vídeo (pk/sk).
- [ ] Status do vídeo atualizado (ex.: PROCESSING).
- [ ] Política de falha no update pós-StartExecution definida e implementada (log/retry ou consistência eventual).

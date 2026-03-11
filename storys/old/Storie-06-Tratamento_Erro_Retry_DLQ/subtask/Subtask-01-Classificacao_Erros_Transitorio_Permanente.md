# Subtask 01: Classificação de erros (transitório vs permanente)

## Descrição
Definir e implementar a classificação de exceções e falhas em transitórias (retry) vs permanentes (não retry, encaminhar para DLQ após maxReceiveCount).

## Passos de implementação
1. Listar exceções e cenários: ThrottlingException, ServiceUnavailable, Timeout, erro de rede → transitório; ValidationException (payload), ArgumentException (validação), ConditionalCheckFailed (já tratado como idempotência) → permanente ou sucesso.
2. Implementar função ou serviço que, dado uma exceção ou resultado de falha, retorna "transitório" ou "permanente".
3. Documentar a tabela de classificação no README ou em docs/.

## Formas de teste
1. Testes unitários: para cada tipo de exceção conhecida, verificar classificação esperada.
2. Casos de borda: exceção genérica (Exception) → definir política (ex.: tratar como transitório por segurança).
3. Revisão: alinhamento com boas práticas AWS (retry apenas para erros retentáveis).

## Critérios de aceite da subtask
- [ ] Classificação transitório vs permanente implementada e documentada.
- [ ] Exceções AWS conhecidas (Throttling, ServiceUnavailable, etc.) mapeadas.
- [ ] Erros de validação/payload tratados como permanentes (ou sucesso idempotente quando aplicável).

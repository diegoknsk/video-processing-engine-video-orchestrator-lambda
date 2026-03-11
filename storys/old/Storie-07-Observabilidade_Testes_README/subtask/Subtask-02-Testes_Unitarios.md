# Subtask 02: Planejamento e implementação de testes unitários

## Descrição
Planejar e implementar testes unitários para os componentes críticos: parsing (SNS/S3), validação, idempotência (update condicional e interpretação de ConditionalCheckFailed), classificação de erros (transitório/permanente) e construção de SQSBatchResponse.

## Passos de implementação
1. Listar cenários de teste: parsing com payload válido e inválido; validação com campos faltando; idempotência (primeira vez vs já processado); classificação de exceções; batch com sucesso parcial e batchItemFailures.
2. Implementar testes usando xUnit (ou NUnit) e mocks (Moq ou NSubstitute) para DynamoDB, Step Functions e SQS events.
3. Garantir que `dotnet test` execute todos os testes e passe; documentar cobertura mínima desejada (ex.: ≥ 80% para core).

## Formas de teste
1. Executar `dotnet test` e verificar que todos os testes passam.
2. Revisão: cenários cobrem caminhos felizes e falhas (payload inválido, condição idempotência falha, erros transitórios).
3. Opcional: relatório de cobertura e verificação de que componentes críticos estão cobertos.

## Critérios de aceite da subtask
- [ ] Testes unitários implementados para parsing, validação, idempotência, classificação de erros e SQSBatchResponse.
- [ ] `dotnet test` passa em todos os testes.
- [ ] Cobertura mínima documentada ou atendida para componentes críticos.

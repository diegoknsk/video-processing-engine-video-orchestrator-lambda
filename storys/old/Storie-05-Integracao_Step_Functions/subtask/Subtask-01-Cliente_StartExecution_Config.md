# Subtask 01: Cliente StartExecution e configuração (ARN, nome)

## Descrição
Implementar o cliente (ou wrapper) que chama a API StartExecution da Step Functions, com State Machine ARN e nome de execução configuráveis via variáveis de ambiente (ou configuração).

## Passos de implementação
1. Definir variáveis de ambiente (ex.: STATE_MACHINE_ARN, EXECUTION_NAME_PREFIX ou nome gerado com correlationId/videoId).
2. Implementar chamada StartExecution usando AWS SDK (AmazonStepFunctionsClient), com StateMachineArn e Name (e input será adicionado na Subtask 02).
3. Tratar exceções da API (ValidationException, ThrottlingException, etc.) para permitir retry ou DLQ conforme Storie-06.

## Formas de teste
1. Teste unitário com mock do cliente: verificação de que ARN e nome são usados corretamente.
2. Documentar no README as variáveis STATE_MACHINE_ARN e nome de execução.
3. Teste de integração (opcional) contra State Machine real ou mock HTTP.

## Critérios de aceite da subtask
- [ ] Cliente/wrapper de Step Functions implementado.
- [ ] ARN e nome de execução configuráveis (variáveis de ambiente ou config).
- [ ] Exceções da API tratadas (não deixar exceção não tratada).

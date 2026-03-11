# Subtask 04: README técnico (variáveis de ambiente, fluxo)

## Descrição
Escrever ou atualizar o README técnico do repositório com: variáveis de ambiente necessárias para o Lambda (STATE_MACHINE_ARN, tabela DynamoDB, etc.), descrição do fluxo do Orchestrator (SQS → parsing → idempotência → Step Functions → DynamoDB), comandos de build e teste, e referência à DLQ e observabilidade.

## Passos de implementação
1. Listar todas as variáveis de ambiente (ou configuração) usadas pelo Lambda: STATE_MACHINE_ARN, nome da tabela DynamoDB, região, etc.
2. Descrever o fluxo em texto ou diagrama: evento SQS (SNS + S3) → validação → idempotência (DynamoDB) → StartExecution → persistência stepExecutionArn → resposta batch.
3. Incluir seções: pré-requisitos (.NET 10), build, testes, deploy (referência ao pipeline), configuração (env vars), DLQ e logs (CloudWatch, correlationId).

## Formas de teste
1. Revisão: outro desenvolvedor consegue clonar, fazer build, rodar testes e entender o fluxo.
2. Verificar que variáveis de ambiente estão documentadas e que não falta nenhuma crítica.
3. Validar que comandos de build e teste estão corretos no README.

## Critérios de aceite da subtask
- [ ] README contém lista de variáveis de ambiente necessárias.
- [ ] Fluxo do Orchestrator descrito (SQS → parsing → idempotência → Step Functions → DynamoDB).
- [ ] Comandos de build e teste documentados e funcionais.

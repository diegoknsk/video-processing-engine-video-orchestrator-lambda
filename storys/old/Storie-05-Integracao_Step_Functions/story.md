# Storie-05: Integração Step Functions (StartExecution + persistência stepExecutionArn)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como Orchestrator do Video Processing Engine, quero sempre iniciar uma execução da State Machine (Step Functions) para cada vídeo válido e idempotente e persistir o stepExecutionArn no DynamoDB, para que o processamento seja sempre fan-out/fan-in com degeneração natural para 1 chunk quando o vídeo for pequeno.

## Objetivo
Implementar a chamada StartExecution da Step Functions com o input definido na Storie-03, obter o executionArn (stepExecutionArn) da resposta e persistir no DynamoDB (atualizar o item USER#userId / VIDEO#videoId com status e stepExecutionArn). O Orchestrator não decide SINGLE vs FANOUT; a State Machine calcula os chunks.

## Escopo Técnico
- Tecnologias: .NET 10, AWS SDK for .NET (Step Functions), DynamoDB (update com stepExecutionArn)
- Arquivos afetados: cliente/port Step Functions, repositório DynamoDB (update pós-StartExecution), handler
- Componentes: StartExecution com input JSON, persistência de executionArn e status no item do vídeo
- Pacotes/Dependências: AWSSDK.StepFunctions (versão compatível .NET 10), AWSSDK.DynamoDBv2

## Dependências e Riscos (para estimativa)
- Dependências: Storie-03 (modelo de input), Storie-04 (idempotência e update condicional).
- Riscos: ARN da State Machine e permissões IAM; pré-condição: State Machine criada e nome/ARN configurável (variável de ambiente).

## Subtasks
- [Subtask 01: Cliente StartExecution e configuração (ARN, nome)](./subtask/Subtask-01-Cliente_StartExecution_Config.md)
- [Subtask 02: Chamar StartExecution com input e obter executionArn](./subtask/Subtask-02-Chamar_StartExecution_Input.md)
- [Subtask 03: Persistir stepExecutionArn e status no DynamoDB](./subtask/Subtask-03-Persistir_StepExecutionArn_DynamoDB.md)

## Critérios de Aceite da História
- [ ] Orchestrator sempre chama StartExecution para cada mensagem válida e idempotente (não "já processado")
- [ ] Input da State Machine contém videoId, bucket, key, userId (e correlationId/requestId quando aplicável)
- [ ] stepExecutionArn (executionArn retornado) é persistido no DynamoDB no item correto (pk/sk)
- [ ] Status do vídeo atualizado no DynamoDB (ex.: PROCESSING ou equivalente) após StartExecution bem-sucedido
- [ ] Nenhuma lógica de SINGLE vs FANOUT ou cálculo de chunks no Orchestrator; responsabilidade da State Machine
- [ ] Observabilidade: log com executionArn e correlationId após sucesso

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

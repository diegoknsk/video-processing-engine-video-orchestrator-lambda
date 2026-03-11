# Subtask 02: Chamar StartExecution com input e obter executionArn

## Descrição
Montar o input JSON para a State Machine (modelo da Storie-03), chamar StartExecution com esse input e obter da resposta o executionArn (stepExecutionArn) para persistência.

## Passos de implementação
1. Serializar o modelo de input (StepFunctionInput) para JSON e passar em StartExecutionRequest.Input.
2. Chamar StartExecution e ler StartExecutionResponse.ExecutionArn.
3. Retornar executionArn (e eventualmente StartDate) para o fluxo persistir no DynamoDB.

## Formas de teste
1. Teste unitário: dado input válido, serialização produz JSON esperado e mock de resposta retorna executionArn.
2. Verificar que o input não contém lógica de chunks (apenas dados do vídeo e contexto).
3. Teste de integração (opcional): StartExecution real e verificação do ARN retornado.

## Critérios de aceite da subtask
- [ ] Input da Storie-03 serializado e enviado em StartExecution.
- [ ] executionArn obtido da resposta e exposto ao fluxo.
- [ ] Nenhum cálculo de chunks ou decisão SINGLE/FANOUT no Orchestrator.

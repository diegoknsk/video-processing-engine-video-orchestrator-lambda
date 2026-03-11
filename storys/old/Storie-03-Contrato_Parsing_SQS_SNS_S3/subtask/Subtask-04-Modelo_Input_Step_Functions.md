# Subtask 04: Modelo mínimo de input para Step Functions

## Descrição
Definir o modelo mínimo de input que o Orchestrator enviará para a State Machine (StartExecution): videoId, bucket, key, userId e demais campos necessários para a State Machine calcular chunks e executar o fan-out (degenerando para 1 chunk quando pequeno). Não implementar a chamada StartExecution nesta story.

## Passos de implementação
1. Documentar o contrato de input da State Machine (campos e tipos) acordado com o time ou com a definição da State Machine.
2. Criar DTO ou classe de input para Step Functions (ex.: StepFunctionInput) com videoId, bucket, key, userId (e correlationId, requestId se aplicável).
3. Mapear do modelo interno (após parsing e validação) para StepFunctionInput; garantir que todos os campos obrigatórios estejam preenchidos.

## Formas de teste
1. Teste unitário: dado modelo interno válido, mapeamento produz StepFunctionInput com todos os campos preenchidos.
2. Revisão: input não contém lógica de chunks (responsabilidade da State Machine).
3. Documentação do contrato disponível para integração na Storie-05.

## Critérios de aceite da subtask
- [ ] Modelo de input para Step Functions definido e documentado.
- [ ] Mapeamento do modelo interno para StepFunctionInput implementado.
- [ ] Input não inclui cálculo de chunks (fan-out é responsabilidade da State Machine).

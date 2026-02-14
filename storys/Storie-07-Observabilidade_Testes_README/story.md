# Storie-07: Observabilidade, testes e README técnico

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor e operador do Video Orchestrator, quero logs estruturados com correlationId, testes unitários cobrindo o fluxo principal, um teste BDD planejado (exigência do hackathon) e README técnico com variáveis de ambiente e fluxo, para garantir rastreabilidade, qualidade e onboarding.

## Objetivo
Implementar e consolidar observabilidade (logs estruturados com correlationId em todo o fluxo), planejar e implementar testes unitários para os componentes críticos (parsing, idempotência, handler, classificação de erros), planejar um cenário BDD (SpecFlow ou equivalente) para um fluxo de ponta a ponta, e documentar no README técnico as variáveis de ambiente, o fluxo do Orchestrator e como executar build/testes.

## Escopo Técnico
- Tecnologias: .NET 10, Microsoft.Extensions.Logging (ou Lambda Logger), xUnit (ou NUnit), SpecFlow/BDD (planejamento), System.Text.Json
- Arquivos afetados: projeto de testes, cenários BDD, README, pontos de log no handler e serviços
- Componentes: logging estruturado (correlationId, requestId, videoId, executionArn), testes unitários, feature/cenário BDD, README
- Pacotes/Dependências: xUnit (ou NUnit), Moq ou NSubstitute, SpecFlow (para BDD), pacotes de logging já em uso

## Dependências e Riscos (para estimativa)
- Dependências: Storie-01 a Storie-06 (fluxo completo implementado).
- Riscos: nenhum crítico; pré-condição: convenção de correlationId já utilizada no handler (Storie-01/02).

## Subtasks
- [Subtask 01: Logs estruturados com correlationId em todo o fluxo](./subtask/Subtask-01-Logs_Estruturados_CorrelationId.md)
- [Subtask 02: Planejamento e implementação de testes unitários](./subtask/Subtask-02-Testes_Unitarios.md)
- [Subtask 03: Planejamento de um teste BDD (SpecFlow)](./subtask/Subtask-03-Teste_BDD_Planejado.md)
- [Subtask 04: README técnico (variáveis de ambiente, fluxo)](./subtask/Subtask-04-README_Tecnico.md)

## Critérios de Aceite da História
- [ ] Logs estruturados incluem correlationId (e requestId, videoId, stepExecutionArn quando aplicável) em pontos-chave do fluxo
- [ ] Testes unitários cobrindo parsing, validação, idempotência (update condicional), classificação de erros e construção de SQSBatchResponse; build de testes passa
- [ ] Um cenário BDD planejado e implementado (ex.: dado evento S3 válido, quando Orchestrator processa, então Step Execution é iniciada e DynamoDB atualizado) conforme exigência do hackathon
- [ ] README técnico contém: variáveis de ambiente necessárias, descrição do fluxo (SQS → parsing → idempotência → Step Functions → DynamoDB), comandos de build e teste
- [ ] Observabilidade: falhas e sucessos são logados de forma a permitir rastreio por correlationId no CloudWatch

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

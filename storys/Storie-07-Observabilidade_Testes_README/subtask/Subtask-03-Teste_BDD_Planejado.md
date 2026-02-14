# Subtask 03: Planejamento de um teste BDD (SpecFlow)

## Descrição
Planejar e implementar um cenário BDD (SpecFlow ou equivalente) para o Video Orchestrator, conforme exigência do hackathon: dado um evento (ex.: mensagem SQS com SNS e S3), quando o Orchestrator processa, então verificar um resultado observável (ex.: Step Execution iniciada, DynamoDB atualizado, ou resposta batch esperada).

## Passos de implementação
1. Escolher ferramenta (SpecFlow com xUnit/NUnit) e criar projeto ou pasta de testes BDD.
2. Escrever uma feature e um cenário em Gherkin (Dado/Quando/Então) que descreva um fluxo de sucesso: evento válido → processamento → resultado esperado (mock de Step Functions e DynamoDB ou integração leve).
3. Implementar os step definitions que executam o handler (ou orquestrador) com mocks e verificam o resultado (ex.: StartExecution chamado com input correto; DynamoDB update chamado com stepExecutionArn).

## Formas de teste
1. Executar o cenário BDD (`dotnet test` no projeto SpecFlow) e verificar que passa.
2. Revisão: linguagem de negócio clara no Gherkin; steps implementados de forma maintainável.
3. Documentar no README como rodar o teste BDD.

## Critérios de aceite da subtask
- [ ] Um cenário BDD implementado (feature + steps) para o fluxo do Orchestrator.
- [ ] Cenário passa quando executado com mocks ou ambiente controlado.
- [ ] Atende à exigência do hackathon de ter pelo menos um teste BDD.

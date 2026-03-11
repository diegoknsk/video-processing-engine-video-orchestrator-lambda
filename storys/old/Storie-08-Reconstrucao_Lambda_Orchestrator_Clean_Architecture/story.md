# Storie-08: Reconstrução do Lambda Orchestrator com Clean Architecture

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do time, quero reconstruir do zero o projeto `VideoOrchestrator` seguindo a mesma arquitetura limpa utilizada no `LambdaUpdateVideo`, para ter uma base sólida, testável localmente com AWS Lambda Test Tool e preparada para receber eventos SQS.

## Objetivo
Remover o projeto `src/VideoOrchestrator` atual e criar uma nova solução multi-camadas (`Domain`, `Application`, `Infra.CrossCutting`, `Infra.Data` e `InterfacesExternas/Lambda`) com resposta totalmente mockada, capaz de receber eventos SQS (envelope SQS ou JSON direto) e retornar uma resposta de echo, testável localmente via `dotnet-lambda-test-tool-10.0`.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, AWS Lambda, SQS
- **Arquivos afetados:**
  - Removido: `src/VideoOrchestrator/` (projeto existente inteiro)
  - Criados:
    - `src/Core/VideoProcessing.VideoOrchestrator.Domain/`
    - `src/Core/VideoProcessing.VideoOrchestrator.Application/`
    - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/`
    - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/`
    - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/`
    - `tests/VideoProcessing.VideoOrchestrator.UnitTests/`
    - `VideoOrchestrator.slnx` (ou `.sln`) na raiz do src
- **Componentes criados:**
  - `Domain`: sem dependências externas — `OrchestratorJob` (entidade mínima), `JobStatus` (enum)
  - `Application`: `IOrchestratorJobHandler` (port), `IOrchestratorJobUseCase` (interface do use case), `OrchestratorJobInputModel`, `OrchestratorJobResponseModel`, use case mockado
  - `Infra.CrossCutting`: `Startup`/bootstrap, logging via console
  - `Infra.Data`: stub de repositório (sem DynamoDB nessa fase)
  - `Lambda`: `Function.cs`, `Startup.cs`, `IOrchestratorEventAdapter` / `OrchestratorEventAdapter`, `IOrchestratorHandler` / `OrchestratorHandler`, `Models/OrchestratorLambdaEvent`, `Models/OrchestratorLambdaResponse`, `aws-lambda-tools-defaults.json`, `Properties/launchSettings.json`
  - `UnitTests`: cobertura de `OrchestratorEventAdapter` e `OrchestratorHandler`
- **Pacotes/Dependências:**
  - `Amazon.Lambda.Core` 2.8.0
  - `Amazon.Lambda.Serialization.SystemTextJson` 2.4.5
  - `Microsoft.Extensions.Configuration` 10.0.x
  - `Microsoft.Extensions.Configuration.EnvironmentVariables` 10.0.x
  - `Microsoft.Extensions.DependencyInjection` 10.0.x
  - `Microsoft.Extensions.Logging.Console` 10.0.x
  - `Microsoft.Extensions.Options.ConfigurationExtensions` 10.0.x
  - `xunit` 2.9.2 (testes)
  - `Moq` 4.20.72 (testes)
  - `FluentAssertions` 6.12.0 (testes)
  - `coverlet.collector` 6.0.4 (testes)
  - `Microsoft.NET.Test.Sdk` (testes)

## Dependências e Riscos (para estimativa)
- **Dependências:** Nenhuma story anterior bloqueante; o projeto atual será removido e recriado do zero.
- **Riscos/Pré-condições:**
  - `dotnet-lambda-test-tool-10.0` deve estar instalado globalmente (`dotnet tool install -g Amazon.Lambda.TestTool --version 0.14.x`)
  - O `.sln`/`.slnx` da raiz pode precisar ser atualizado para referenciar os novos projetos
  - A pipeline GitHub Actions (Storie-09) será atualizada separadamente; nessa story o foco é só o código local

## Subtasks
- [Subtask 01: Remover projeto existente e criar estrutura de pastas e projetos .NET](./subtask/Subtask-01-Remover_Existente_Criar_Estrutura.md)
- [Subtask 02: Implementar Domain e Application layers (contratos e use case mockado)](./subtask/Subtask-02-Domain_Application_Layers.md)
- [Subtask 03: Implementar Infra.CrossCutting e Infra.Data (stubs)](./subtask/Subtask-03-Infra_CrossCutting_Data_Stubs.md)
- [Subtask 04: Implementar Lambda entry point — Function, Startup, EventAdapter e Handler](./subtask/Subtask-04-Lambda_Function_Startup_Adapter_Handler.md)
- [Subtask 05: Configurar aws-lambda-tools-defaults.json e launchSettings.json para teste local](./subtask/Subtask-05-Config_Lambda_Test_Tool_Local.md)
- [Subtask 06: Criar projeto de testes unitários e cobertura mínima](./subtask/Subtask-06-Testes_Unitarios.md)
- [Subtask 07: Validar build local, testes passando e execução no Lambda Test Tool](./subtask/Subtask-07-Validacao_Build_Teste_Local.md)

## Critérios de Aceite da História
- [ ] O diretório `src/VideoOrchestrator` antigo foi completamente removido
- [ ] Estrutura de projetos criada: `Domain`, `Application`, `Infra.CrossCutting`, `Infra.Data`, `Lambda` e `UnitTests`, com namespaces `VideoProcessing.VideoOrchestrator.*`
- [ ] `dotnet build` na solução completa retorna zero erros e zero warnings relevantes
- [ ] Lambda aceita payload SQS (envelope `Records[].body`) e também JSON direto, via `OrchestratorEventAdapter`
- [ ] A resposta é 100% mockada: retorna `statusCode: 200` com echo do `correlationId` e `jobId` recebidos
- [ ] `Properties/launchSettings.json` contém perfil `Lambda Test Tool` apontando para `dotnet-lambda-test-tool-10.0.exe`
- [ ] `aws-lambda-tools-defaults.json` contém `function-handler` correto no formato `Assembly::Namespace.Class::Method`
- [ ] `dotnet test` passa com cobertura ≥ 80% nos projetos de teste
- [ ] Injeção de dependência via `Startup.BuildServiceProvider()` (sem `AddAWSLambdaHosting`), igual ao padrão `LambdaUpdateVideo`
- [ ] Serialização via `[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]` em `Function.cs`

## Rastreamento (dev tracking)
- **Início:** 11/03/2026, às 00:30 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —

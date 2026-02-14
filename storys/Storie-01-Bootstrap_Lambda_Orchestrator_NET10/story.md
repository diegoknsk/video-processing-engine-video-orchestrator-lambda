# Storie-01: Bootstrap Lambda Orchestrator em .NET 10

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do Video Processing Engine, quero um projeto Lambda .NET 10 com handler puro e estrutura mínima validada, para ter a base do Video Orchestrator sem integrações reais ainda.

## Objetivo
Criar o projeto .NET 10 do Lambda Video Orchestrator com handler puro, entry point correto para AWS Lambda, logging estruturado básico, estrutura mínima de pastas e build local validado. Sem integrações com SQS, DynamoDB ou Step Functions.

## Escopo Técnico
- Tecnologias: .NET 10 (SDK 10.0.x), C# 13, AWS Lambda .NET Runtime
- Arquivos afetados: raiz do repositório (novo projeto), `src/` ou equivalente conforme convenção
- Componentes: projeto de biblioteca/executável Lambda, classe handler, entry point assembly
- Pacotes/Dependências:
  - Amazon.Lambda.Core (versão compatível .NET 10)
  - Amazon.Lambda.RuntimeSupport (se aplicável)
  - Microsoft.Extensions.Logging (ou logging mínimo sem AddAWSLambdaHosting)

## Dependências e Riscos (para estimativa)
- Dependências: nenhuma (primeira story).
- Riscos: garantir que o SDK 10.0.x esteja disponível no ambiente de build; pré-condição: máquina com .NET 10 SDK instalado.

## Subtasks
- [Subtask 01: Criar projeto .NET 10 e estrutura de pastas](./subtask/Subtask-01-Projeto_DotNet_10_Estrutura.md)
- [Subtask 02: Handler puro e EntryPoint para AWS Lambda](./subtask/Subtask-02-Handler_Puro_EntryPoint.md)
- [Subtask 03: Logging estruturado básico](./subtask/Subtask-03-Logging_Estruturado_Basico.md)
- [Subtask 04: Build local validado](./subtask/Subtask-04-Build_Local_Validado.md)

## Critérios de Aceite da História
- [ ] Projeto criado com target framework .NET 10 (net10.0 ou equivalente) e SDK 10.0.x
- [ ] Handler puro exposto (sem AddAWSLambdaHosting); assembly/entry point configurado para Lambda
- [ ] Estrutura mínima de pastas documentada ou evidente (ex.: Handler, modelos mínimos)
- [ ] Logging estruturado básico presente (ex.: requestId, mensagem de início/fim)
- [ ] `dotnet build` e `dotnet publish` executam com sucesso localmente
- [ ] Nenhuma integração real com SQS, DynamoDB ou Step Functions ainda

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

# Storie-01: Ajustar Deploy do Lambda para Ambiente Correto

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como engenheiro responsável pelo pipeline de CI/CD, quero corrigir o workflow de deploy da Lambda, para que o código seja publicado no function correto da AWS e a referência de projeto aponte para o path atual.

## Objetivo
Corrigir o workflow `.github/workflows/deploy-lambda.yml` para que utilize o path correto do projeto `.csproj` (após reestruturação de pastas), garantir que o `function-name` da Lambda seja o real desta aplicação (substituindo o valor de teste deixado por outro projeto) e incluir o step de atualização do handler (`aws lambda update-function-configuration --handler`) com os devidos waits entre as operações.

## Escopo Técnico
- Tecnologias: GitHub Actions, AWS CLI, .NET 10, AWS Lambda
- Arquivos afetados:
  - `.github/workflows/deploy-lambda.yml`
  - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/aws-lambda-tools-defaults.json`
- Componentes/Recursos: workflow de deploy, secret `AWS_LAMBDA_FUNCTION_NAME` no repositório GitHub
- Handler correto: `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::FunctionHandler`
- Pacotes/Dependências: nenhum pacote externo novo

## Dependências e Riscos (para estimativa)
- Dependências: acesso às configurações de Secrets do repositório GitHub; function já criada na AWS
- Riscos/Pré-condições:
  - O function name real da Lambda precisa ser confirmado antes de configurar o secret
  - O secret `AWS_LAMBDA_FUNCTION_NAME` no GitHub deve ser atualizado com o nome correto
  - Caminho antigo `src/VideoOrchestrator/VideoOrchestrator.csproj` está deletado no git

## Subtasks
- [x] [Subtask 01: Corrigir path do projeto no workflow](./subtask/Subtask-01-Corrigir_Path_Projeto_Workflow.md)
- [x] [Subtask 02: Atualizar function-name no aws-lambda-tools-defaults](./subtask/Subtask-02-Atualizar_FunctionName_Defaults.md)
- [ ] [Subtask 03: Atualizar secret AWS_LAMBDA_FUNCTION_NAME no GitHub](./subtask/Subtask-03-Atualizar_Secret_GitHub.md) _(manual: Settings → Secrets and variables → Actions)_
- [ ] [Subtask 04: Validar pipeline end-to-end](./subtask/Subtask-04-Validar_Pipeline.md) _(manual: workflow_dispatch ou push em main/dev)_

## Critérios de Aceite da História
- [ ] A env var `LAMBDA_PROJECT` no workflow aponta para `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`
- [ ] O step `Restore` e `Build` executam sem erro referenciando o projeto correto
- [ ] O step `Deploy to Lambda` utiliza o function name real (não o de teste do outro projeto)
- [ ] O secret `AWS_LAMBDA_FUNCTION_NAME` no repositório GitHub está configurado com o nome correto da função
- [ ] O campo `function-name` no `aws-lambda-tools-defaults.json` está preenchido com o nome real da função
- [ ] O workflow possui step de `Wait for code update` (aws lambda wait function-updated) após o update de código
- [ ] O workflow possui step `Set Lambda handler` usando `aws lambda update-function-configuration --handler` com o valor `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::FunctionHandler`
- [ ] O workflow possui step de `Wait for configuration update` após o update de configuração
- [ ] O pipeline de deploy é executado com sucesso em push para `main` ou `dev`
- [ ] A Lambda na AWS reflete o código do projeto `VideoProcessing.VideoOrchestrator.Lambda` após o deploy

## Rastreamento (dev tracking)
- **Início:** 11/03/2026, às 20:08 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —

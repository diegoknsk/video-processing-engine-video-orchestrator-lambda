# Storie-09: Atualização do Pipeline GitHub Actions para Deploy do Novo Projeto

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do time, quero atualizar o workflow `.github/workflows/deploy-lambda.yml` para apontar ao novo projeto `VideoProcessing.VideoOrchestrator.Lambda` (criado na Storie-08), para que o pipeline de CI/CD construa, publique e faça deploy correto da nova Lambda no AWS.

## Objetivo
Atualizar o workflow existente de GitHub Actions para:
1. Apontar para o novo `.csproj` em `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/`
2. Adicionar um step de atualização do `--handler` na função Lambda (via `aws lambda update-function-configuration`) para refletir o novo handler `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`
3. Manter os mesmos secrets existentes (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`, `AWS_LAMBDA_FUNCTION_NAME`) sem alterações
4. Garantir que o pipeline continua funcionando em push para `main` e `dev`, e via `workflow_dispatch`

## Escopo Técnico
- **Tecnologias:** GitHub Actions, AWS CLI, .NET 10, AWS Lambda
- **Arquivos afetados:**
  - `.github/workflows/deploy-lambda.yml` — atualização da variável `LAMBDA_PROJECT` e adição do step de update handler
- **Componentes/Recursos:**
  - Variável `LAMBDA_PROJECT` atualizada para novo caminho
  - Novo step `aws lambda update-function-configuration --handler` após o deploy do código
- **Pacotes/Dependências:**
  - `actions/checkout@v4` (sem mudança)
  - `actions/setup-dotnet@v5` (sem mudança)
  - `aws-actions/configure-aws-credentials@v4` (sem mudança)
  - AWS CLI (disponível nativamente no `ubuntu-latest`)

## Dependências e Riscos (para estimativa)
- **Dependências:** Storie-08 deve estar concluída — o novo projeto `.csproj` deve existir antes de atualizar o pipeline
- **Riscos/Pré-condições:**
  - Os secrets `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION` e `AWS_LAMBDA_FUNCTION_NAME` devem estar configurados no repositório GitHub
  - A função Lambda deve existir no AWS (criada previamente via console ou IaC) antes do primeiro deploy
  - O runtime da Lambda no AWS deve estar configurado como `dotnet10` com arquitetura `x86_64`
  - O IAM role configurado nos secrets deve ter permissão `lambda:UpdateFunctionCode` e `lambda:UpdateFunctionConfiguration`

## Subtasks
- [Subtask 01: Atualizar variável LAMBDA_PROJECT e caminho de publish no workflow](./subtask/Subtask-01-Atualizar_LAMBDA_PROJECT_Workflow.md)
- [Subtask 02: Adicionar step de atualização do handler na função Lambda](./subtask/Subtask-02-Step_Update_Handler_Lambda.md)
- [Subtask 03: Validar pipeline — dry-run local e execução via workflow_dispatch](./subtask/Subtask-03-Validacao_Pipeline_Deploy.md)

## Critérios de Aceite da História
- [ ] Variável `LAMBDA_PROJECT` aponta para `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`
- [ ] Step de `dotnet publish` usa o novo caminho do projeto com `-r linux-x64 --self-contained false`
- [ ] Step de `aws lambda update-function-configuration --handler` atualiza o handler para `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`
- [ ] Os 4 secrets existentes (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`, `AWS_LAMBDA_FUNCTION_NAME`) são mantidos sem alteração
- [ ] Pipeline executa com sucesso via `workflow_dispatch` sem erros de build, publish ou deploy
- [ ] Após deploy, a função Lambda no AWS responde com `statusCode: 200` ao receber um evento SQS de teste
- [ ] O workflow continua com trigger em push para `main` e `dev`

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

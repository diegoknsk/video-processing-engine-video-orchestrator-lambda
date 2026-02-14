# Storie-02: Mock + Deploy (.NET 10) + Prova de Pipeline

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do Video Processing Engine, quero que o Lambda retorne uma resposta MOCK e seja implantado via pipeline (GitHub Actions), para comprovar que o Lambda sobe, executa e que o pipeline de deploy está funcional.

## Objetivo
Fazer o handler retornar uma resposta MOCK básica (ex.: status=ok, requestId, correlationId), criar o GitHub Action com setup-dotnet versão 10, pipeline de restore/build/publish/zip/deploy no Lambda, autenticação AWS via variáveis de ambiente, e planejar validação via deploy e CloudWatch Logs.

## Escopo Técnico
- Tecnologias: .NET 10, GitHub Actions, AWS Lambda, AWS CLI ou SDK para deploy
- Arquivos afetados: `.github/workflows/`, handler (resposta mock), documentação de teste de deploy
- Componentes: workflow YAML, handler com resposta mock, script ou passo de zip e deploy
- Pacotes/Dependências: (build) .NET 10 SDK; (deploy) credenciais AWS (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN quando aplicável)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-01 concluída (projeto e handler existentes).
- Riscos: credenciais AWS em secrets do GitHub; pré-condição: função Lambda já criada na conta (ou criação via IaC no pipeline).

## Subtasks
- [Subtask 01: Resposta MOCK no handler](./subtask/Subtask-01-Resposta_Mock_Handler.md)
- [Subtask 02: GitHub Action com setup-dotnet 10](./subtask/Subtask-02-GitHub_Action_DotNet_10.md)
- [Subtask 03: Pipeline restore, build, publish, zip e deploy Lambda](./subtask/Subtask-03-Pipeline_Deploy_Lambda.md)
- [Subtask 04: Planejamento de validação de deploy e CloudWatch Logs](./subtask/Subtask-04-Validacao_Deploy_CloudWatch.md)

## Critérios de Aceite da História
- [ ] Handler retorna resposta MOCK contendo status (ex.: ok), requestId e correlationId (ou equivalente)
- [ ] GitHub Action usa setup-dotnet com versão 10 (10.x ou 10.0.x)
- [ ] Pipeline executa restore, build, publish, geração de zip e deploy na função Lambda
- [ ] Autenticação AWS configurada via AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY e AWS_SESSION_TOKEN (quando aplicável)
- [ ] Documentado como testar o deploy e como validar execução via CloudWatch Logs
- [ ] Objetivo atingido: comprovar que o Lambda sobe e executa após o pipeline

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

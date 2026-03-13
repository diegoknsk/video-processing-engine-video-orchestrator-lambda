# Storie-01: Integração SonarCloud com Cobertura de Código via GitHub Actions

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como time de engenharia, quero integrar o SonarCloud com cobertura de código OpenCover ao pipeline de CI/CD via GitHub Actions, para garantir qualidade de código mensurável e bloquear merges para `main` que não atendam ao Quality Gate.

## Objetivo
Configurar análise estática contínua com SonarCloud — incluindo cobertura de testes, Quality Gate obrigatório em PRs para `main`, e badges de status no README — de forma que o deploy para Lambda seja bloqueado enquanto a análise não passar.

## Escopo Técnico
- **Tecnologias:** .NET 10, GitHub Actions, SonarCloud, coverlet
- **Arquivos afetados:**
  - `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/VideoProcessing.VideoOrchestrator.UnitTests.csproj`
  - `.github/workflows/deploy-lambda.yml`
  - `.gitignore`
  - `README.md`
- **Componentes/Recursos:**
  - Job `sonar-analysis` no workflow existente
  - Dependência `needs: [sonar-analysis]` no job `build-and-deploy`
  - Trigger `pull_request` adicionado ao workflow para PRs direcionados a `main`
- **Pacotes/Dependências:**
  - coverlet.collector (6.0.2)
  - coverlet.msbuild (6.0.2)

## Dependências e Riscos (para estimativa)
- **Dependências:** Conta SonarCloud vinculada à organização GitHub; repositório deve ser público ou ter licença SonarCloud para privado.
- **Riscos/Pré-condições:**
  - Automatic Analysis deve ser desativada no SonarCloud **antes** de rodar o pipeline CI (armadilha crítica — causa exit code 1).
  - `sonar.projectBaseDir` deve usar `${{ github.workspace }}` (caminho absoluto), nunca `"."`.
  - Path do relatório de cobertura deve ser relativo ao projeto de testes: `./TestResults/coverage.opencover.xml`.
  - Se houver projeto duplicado no SonarCloud (criado automaticamente), deletar o órfão antes de configurar o CI.

## Subtasks
- [Subtask 01: Adicionar pacotes coverlet ao projeto de testes](./subtask/Subtask-01-Pacotes_Coverlet_Projeto_Testes.md)
- [Subtask 02: Adicionar job sonar-analysis e ajustar trigger no workflow](./subtask/Subtask-02-Job_Sonar_Analysis_Workflow.md)
- [Subtask 03: Configurar Secrets, Variables e Projeto no SonarCloud](./subtask/Subtask-03-Secrets_Variables_SonarCloud.md)
- [Subtask 04: Atualizar .gitignore com entradas do Sonar](./subtask/Subtask-04-Gitignore_Entradas_Sonar.md)
- [Subtask 05: Branch Protection Rule e Badges no README](./subtask/Subtask-05-Branch_Protection_Badges_README.md)

## Critérios de Aceite da História
- [ ] Pacotes `coverlet.collector` e `coverlet.msbuild` (v6.0.2) adicionados ao `.csproj` de testes com `PrivateAssets=all`
- [ ] Job `sonar-analysis` adicionado ao workflow com `fetch-depth: 0` e `sonar.projectBaseDir="${{ github.workspace }}"`
- [ ] Workflow dispara análise SonarCloud em PRs para `main` e em push para `main`
- [ ] Job `build-and-deploy` declara `needs: [sonar-analysis]` e só executa em evento `push` (não em PR)
- [ ] Cobertura OpenCover aparece no painel do SonarCloud após análise da branch `main`
- [ ] Secret `SONAR_TOKEN` e variables `SONAR_PROJECT_KEY` / `SONAR_ORGANIZATION` configurados no GitHub
- [ ] Automatic Analysis desativada no SonarCloud (Administration → Analysis Method)
- [ ] `.gitignore` contém entradas `.sonarqube/`, `.scannerwork/` e `coverage.opencover.xml`
- [ ] Branch Protection Rule em `main` com check obrigatório `SonarCloud Analysis`
- [ ] README contém badges de Quality Gate e Coverage funcionando

## Rastreamento (dev tracking)
- **Início:** dia 13/03/2026, às 12:45 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —

# Storie-02: Configuração, Deploy e Bootstrap do Orquestrador

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como engenheiro da plataforma, quero configurar todas as variáveis de ambiente, o bootstrap de DI e o pipeline de deploy do Lambda Orquestrador, para que as próximas stories possam consumir configurações de forma segura, tipada e sem nenhuma credencial ou ARN hardcoded.

## Objetivo
Entregar a infraestrutura de configuração e bootstrap do orquestrador: leitura de variáveis de ambiente via `IOptions`, DI configurado na Lambda, secrets adicionados ao GitHub Actions e pipeline atualizado para injetar as novas variáveis. Nenhuma story subsequente deverá ler variáveis de ambiente diretamente — tudo passará por `IOptions<T>`.

## Escopo Técnico
- **Tecnologias:** .NET 10, AWS Lambda, C# 13, GitHub Actions
- **Arquivos afetados:**
  - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/Function.cs`
  - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/` (novo `DependencyInjection.cs` e pasta `Settings/`)
  - `.github/workflows/deploy-lambda.yml`
  - `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/`
- **Componentes criados/modificados:**
  - `VideoManagementApiOptions` — BaseUrl e Timeout da API interna (`Infra.CrossCutting/Settings/`)
  - `M2MAuthOptions` — ClientId, Secret e TokenEndpoint M2M (`Infra.CrossCutting/Settings/`)
  - `StepFunctionOptions` — ARN da Step Function (`Infra.CrossCutting/Settings/`)
  - `DependencyInjection.cs` (CrossCutting) — registro centralizado do DI e binding das Options
  - Atualização do `Function.cs` para bootstrapar DI via `ServiceCollection`
- **Pacotes/Dependências:**
  - `Microsoft.Extensions.DependencyInjection` 10.0.0 (Lambda project)
  - `Microsoft.Extensions.Configuration` 10.0.0 (Lambda project)
  - `Microsoft.Extensions.Configuration.EnvironmentVariables` 10.0.0 (Lambda project)
  - `Microsoft.Extensions.Logging` 10.0.0 (Lambda project)
  - `Microsoft.Extensions.Logging.Console` 10.0.0 (Lambda project)
  - `Microsoft.Extensions.Options.ConfigurationExtensions` 10.0.0 (CrossCutting — já presente)

## Dependências e Riscos (para estimativa)
- **Dependências:** Storie-01 (pipeline funcional) deve estar concluída antes desta story.
- **Riscos/Pré-condições:**
  - Todas as variáveis de ambiente devem ser definidas em comum acordo com o repositório de infra antes de adicionar ao GitHub.
  - Lambda .NET 10 sem ASP.NET Core não possui `appsettings.json` automático — a leitura de configuração se dará exclusivamente via variáveis de ambiente (`AddEnvironmentVariables()`).
  - Secrets sensíveis (M2M client secret) devem ser adicionados como **GitHub Secrets** e nunca como variáveis de ambiente não-criptografadas no workflow.

## Subtasks
- [Subtask 01: Definir e documentar variáveis de configuração](./subtask/Subtask-01-Definir_Variaveis_Configuracao.md)
- [Subtask 02: Criar classes Options e configuração tipada](./subtask/Subtask-02-Classes_Options_Configuracao_Tipada.md)
- [Subtask 03: Bootstrapar DI no Function.cs](./subtask/Subtask-03-Bootstrap_DI_Function.md)
- [Subtask 04: Atualizar pipeline GitHub Actions com novos secrets](./subtask/Subtask-04-Pipeline_Secrets_GitHub_Actions.md)
- [Subtask 05: Testes unitários de configuração e DI](./subtask/Subtask-05-Testes_Configuracao_DI.md)

## Critérios de Aceite da História
- [ ] Todas as configurações sensíveis (M2M clientId, M2M secret, Step Function ARN, URL da API Video Management) são lidas exclusivamente de variáveis de ambiente — zero valores hardcoded em código ou workflow YAML.
- [ ] A Lambda inicializa o `ServiceProvider` no construtor de `Function` usando `ServiceCollection` + `AddEnvironmentVariables()`; chamadas subsequentes reutilizam o mesmo container.
- [ ] `IOptions<M2MAuthOptions>`, `IOptions<VideoManagementApiOptions>` e `IOptions<StepFunctionOptions>` são resolvidos corretamente pelo DI a partir das variáveis de ambiente.
- [ ] O workflow `deploy-lambda.yml` injeta todas as novas variáveis de configuração como environment variables via GitHub Secrets no step de deploy; nenhuma variável sensível aparece em texto claro no YAML.
- [ ] Testes unitários validam que a binding de configuração funciona corretamente para valores válidos e que uma `OptionsValidationException` (ou equivalente) é lançada para campos obrigatórios ausentes; cobertura ≥ 80% nos componentes criados.
- [ ] O `README.md` da Lambda ou um documento em `docs/` lista todas as variáveis de ambiente necessárias com descrição e se são obrigatórias.
- [ ] `dotnet build` e `dotnet test` passam sem erros no pipeline após as alterações.

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

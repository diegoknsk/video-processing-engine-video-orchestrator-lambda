# Subtask-03: Implementar Infra.CrossCutting e Infra.Data (stubs)

## Descrição
Criar os projetos de infraestrutura com implementações stub (sem conexão real com AWS DynamoDB ou S3 nessa fase). O objetivo é ter a estrutura de camadas completa e compilável, deixando os projetos preparados para receber implementações reais em stories futuras.

## Passos de Implementação

1. **Infra.CrossCutting — Configurações e logging:**
   Criar `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/`:
   - `Configuration/LambdaOptions.cs` — record com `[Required]` properties: `AwsRegion (string)` (default `"us-east-1"`)
   - `DependencyInjection/CrossCuttingServiceExtensions.cs` — método de extensão `AddCrossCuttingServices(this IServiceCollection services, IConfiguration config)` que registra `LambdaOptions` via `services.Configure<LambdaOptions>(config.GetSection("Lambda"))`

2. **Infra.CrossCutting — Pacotes:**
   - `Microsoft.Extensions.Configuration.Abstractions` 10.0.x
   - `Microsoft.Extensions.Options.ConfigurationExtensions` 10.0.x
   - `Microsoft.Extensions.Options.DataAnnotations` 10.0.x (para validação de Options)

3. **Infra.Data — Stub do repositório (preparado para futuro):**
   Criar `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/`:
   - `Repositories/OrchestratorJobRepository.cs` — classe stub que lança `NotImplementedException` em todos os métodos. Sem dependência de DynamoDB por enquanto.
   - Deixar comentário: `// TODO (Storie futura): implementar com AWSSDK.DynamoDBv2`

4. **Infra.Data — DependencyInjection:**
   - `DependencyInjection/DataServiceExtensions.cs` — método `AddDataServices(this IServiceCollection services)` que apenas retorna `services` (sem registros reais nessa fase)

5. **Infra.Data — Pacotes (mínimos):**
   - `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.x
   - Não adicionar `AWSSDK.DynamoDBv2` ainda (stub não precisa)

## Formas de Teste

1. `dotnet build` nos projetos `Infra.CrossCutting` e `Infra.Data` deve compilar sem erros
2. Verificar que nenhum dos dois projetos referencia `Amazon.DynamoDBv2` ou qualquer SDK AWS pesado
3. Verificar que `LambdaOptions` pode ser instanciado e tem valor default para `AwsRegion`

## Critérios de Aceite

- [ ] `LambdaOptions` criado em `Infra.CrossCutting` com propriedade `AwsRegion` e default `"us-east-1"`
- [ ] Método de extensão `AddCrossCuttingServices` registra `LambdaOptions` via Options pattern
- [ ] `OrchestratorJobRepository.cs` stub criado em `Infra.Data` (sem implementação real, sem pacotes AWS)
- [ ] `dotnet build` em ambos os projetos retorna 0 erros
- [ ] Nenhum pacote AWS SDK pesado (`AWSSDK.DynamoDBv2`, `AWSSDK.S3`) adicionado nessa subtask

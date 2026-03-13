# Subtask-01: Criar OutputOptions com defaults e registrar no DI

## Descrição
Criar a classe `OutputOptions` no projeto `Infra.CrossCutting`, contendo todas as propriedades de configuração relacionadas aos buckets de saída e parâmetros de processamento (`FramesBucket`, `ManifestBucket`, `ZipBucket`, `FrameIntervalSec`, `OrdenaAutomaticamente`) — todas com defaults controlados, sem nenhuma propriedade `[Required]`. Registrar a classe no `DependencyInjection.cs` do `Infra.CrossCutting` como `IOptions<OutputOptions>`.

## Passos de Implementação

1. **Criar `OutputOptions.cs`** em `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/Settings/OutputOptions.cs`:
   - Seção de binding: `"OUTPUT"` (env: `OUTPUT__*`)
   - Propriedades com defaults:
     - `FramesBucket` → default `"video-processing-engine-dev-images"`
     - `ManifestBucket` → default `"video-processing-engine-dev-images"`
     - `ZipBucket` → default `"video-processing-engine-dev-zip"`
     - `FrameIntervalSec` → default `1`
     - `OrdenaAutomaticamente` → default `true`
   - Nenhuma propriedade `[Required]`; usar `init` com valor default no próprio inicializador de propriedade.

2. **Atualizar `DependencyInjection.cs`** no `Infra.CrossCutting`:
   - Adicionar `services.Configure<OutputOptions>(configuration.GetSection(OutputOptions.SectionName))` junto aos demais registros de options.

3. **Adicionar `SectionName` como constante** na classe `OutputOptions` (ex.: `public const string SectionName = "OUTPUT";`), seguindo o padrão já utilizado em `StepFunctionOptions`.

## Formas de Teste

1. **Teste unitário — binding com variáveis definidas:** configurar um `IConfiguration` em memória com valores `OUTPUT:FRAMES_BUCKET`, `OUTPUT:MANIFEST_BUCKET`, `OUTPUT:ZIP_BUCKET`, `OUTPUT:FRAME_INTERVAL_SEC` e `OUTPUT:ORDENA_AUTOMATICAMENTE`; verificar que as propriedades são lidas corretamente.
2. **Teste unitário — defaults sem variáveis:** configurar um `IConfiguration` vazio e verificar que as propriedades assumem os valores default definidos na classe.
3. **Teste de bootstrap (`FunctionBootstrapTests`):** verificar que `IOptions<OutputOptions>` pode ser resolvido do container sem exceção, sem variáveis de ambiente configuradas.

## Critérios de Aceite

- [ ] `OutputOptions` compila sem erros; binding com seção `"OUTPUT"` funciona corretamente com e sem variáveis de ambiente.
- [ ] Todas as 5 propriedades têm defaults válidos; nenhuma propriedade é `[Required]`.
- [ ] `IOptions<OutputOptions>` é resolvível pelo container de DI registrado em `DependencyInjection.cs`.
- [ ] Testes unitários passam: binding com valores explícitos e binding com defaults.

# Subtask-04: Implementar Lambda entry point — Function, Startup, EventAdapter e Handler

## Descrição
Implementar todos os componentes da borda Lambda no projeto `VideoProcessing.VideoOrchestrator.Lambda`, replicando fielmente o padrão do `LambdaUpdateVideo`: `Function` (entry point), `Startup` (bootstrap DI), `OrchestratorEventAdapter` (normaliza SQS ou JSON direto) e `OrchestratorHandler` (borda que valida e delega ao use case).

## Passos de Implementação

1. **Adicionar pacotes ao projeto Lambda:**
   ```xml
   <PackageReference Include="Amazon.Lambda.Core" Version="2.8.0" />
   <PackageReference Include="Amazon.Lambda.Serialization.SystemTextJson" Version="2.4.5" />
   <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.x" />
   <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.x" />
   <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.x" />
   <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.x" />
   <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="10.0.x" />
   ```

2. **Criar `Models/OrchestratorLambdaEvent.cs`:**
   ```csharp
   public record OrchestratorLambdaEvent
   {
       [JsonPropertyName("jobId")]
       public Guid JobId { get; init; }

       [JsonPropertyName("correlationId")]
       public string CorrelationId { get; init; } = string.Empty;

       [JsonPropertyName("payload")]
       public string? Payload { get; init; }
   }
   ```

3. **Criar `Models/OrchestratorLambdaResponse.cs`:**
   Record com `StatusCode (int)`, `JobId (Guid?)`, `CorrelationId (string?)`, `Status (string?)`, `Message (string?)`, `ErrorCode (string?)`, `ErrorMessage (string?)`.
   Factory methods estáticos: `Ok(OrchestratorJobResponseModel)`, `ValidationError(string)`, `NotFound(string?)`.

4. **Criar interface e implementação `IOrchestratorEventAdapter` / `OrchestratorEventAdapter`:**
   - Interface: `IReadOnlyList<OrchestratorLambdaEvent> FromRawEvent(JsonDocument rawEvent)`
   - Implementação (padrão idêntico ao `UpdateVideoEventAdapter`):
     - Detecta envelope SQS via presença de `Records[].body`
     - SQS: deserializa cada `body` como `OrchestratorLambdaEvent` e retorna lista
     - Direto: deserializa root como único `OrchestratorLambdaEvent`
     - Qualquer falha: loga warning e retorna `Array.Empty<OrchestratorLambdaEvent>()`
   - `JsonSerializerOptions`: `PropertyNameCaseInsensitive = true`, `CamelCase`

5. **Criar interface e implementação `IOrchestratorHandler` / `OrchestratorHandler`:**
   - Interface: `Task<OrchestratorLambdaResponse> HandleAsync(OrchestratorLambdaEvent evt, CancellationToken cancellationToken = default)`
   - Implementação (construtor primário com `IOrchestratorJobUseCase` e `ILogger<OrchestratorHandler>`):
     - Mapeia `OrchestratorLambdaEvent` → `OrchestratorJobInputModel`
     - Chama `useCase.ExecuteAsync(...)`
     - Retorna `OrchestratorLambdaResponse.Ok(response)` em caso de sucesso
     - Trata `Exception` genérica como erro interno: loga e retorna `ValidationError`

6. **Criar `Startup.cs`:**
   ```csharp
   public static class Startup
   {
       public static IServiceProvider BuildServiceProvider()
       {
           var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();

           var services = new ServiceCollection();

           services.AddLogging(b =>
           {
               b.AddConfiguration(config.GetSection("Logging"));
               b.AddConsole();
           });

           services.AddCrossCuttingServices(config);
           services.AddApplicationServices();
           services.AddDataServices();

           services.AddSingleton<IOrchestratorHandler, OrchestratorHandler>();
           services.AddSingleton<IOrchestratorEventAdapter, OrchestratorEventAdapter>();

           return services.BuildServiceProvider();
       }
   }
   ```

7. **Criar `Function.cs`:**
   - `[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]` no topo
   - Construtor público (`Function()`) chama `Startup.BuildServiceProvider()`
   - Construtor interno (`Function(IServiceProvider)`) para testes
   - Método `public async Task<OrchestratorLambdaResponse> Handler(Stream rawEventStream, ILambdaContext context)`:
     - `JsonDocument.ParseAsync(rawEventStream)`
     - `_adapter.FromRawEvent(rawEvent)`
     - Se lista vazia: retorna `ValidationError("Payload inválido")`
     - Itera eventos: se resposta >= 400 retorna imediatamente; senão guarda como `lastSuccess`
     - Retorna `lastSuccess`

8. **Configurar `LambdaHandler` no `.csproj`:**
   ```xml
   <LambdaHandler>VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler</LambdaHandler>
   ```

## Formas de Teste

1. `dotnet build` no projeto `Lambda` deve compilar sem erros
2. Injetar `OrchestratorEventAdapter` manualmente e chamar `FromRawEvent` com JSON de envelope SQS — deve retornar lista com 1 evento
3. Injetar `OrchestratorEventAdapter` e chamar com JSON direto — deve retornar lista com 1 evento
4. Chamar `OrchestratorHandler.HandleAsync` com evento válido — deve retornar `StatusCode = 200`

## Critérios de Aceite

- [ ] `Function.cs` com `[assembly: LambdaSerializer]` e dois construtores (público e interno) implementado
- [ ] `OrchestratorEventAdapter` detecta e parseia corretamente envelope SQS (`Records[].body`) e JSON direto
- [ ] `OrchestratorHandler` delega ao use case e retorna `OrchestratorLambdaResponse` sem lógica de negócio
- [ ] `Startup.BuildServiceProvider()` configura DI completo sem `AddAWSLambdaHosting`
- [ ] `LambdaHandler` no `.csproj` aponta para `Function::Handler` no formato correto
- [ ] `dotnet build` retorna 0 erros no projeto Lambda

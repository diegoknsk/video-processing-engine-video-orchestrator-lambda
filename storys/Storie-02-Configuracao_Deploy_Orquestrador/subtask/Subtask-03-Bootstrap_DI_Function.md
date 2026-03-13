# Subtask-03: Bootstrapar DI no Function.cs

## Descrição
Atualizar o `Function.cs` para inicializar o container de DI usando `ServiceCollection` no construtor, construir o `IConfiguration` a partir de variáveis de ambiente e expor o `ServiceProvider` para uso nos handlers. Esta é a base que as stories 03 e 04 utilizarão para resolver os seus serviços.

## Passos de Implementação

1. **Adicionar os pacotes NuGet necessários** ao projeto Lambda (`.csproj`):
   - `Microsoft.Extensions.DependencyInjection` 10.0.0
   - `Microsoft.Extensions.Configuration` 10.0.0
   - `Microsoft.Extensions.Configuration.EnvironmentVariables` 10.0.0
   - `Microsoft.Extensions.Logging` 10.0.0
   - `Microsoft.Extensions.Logging.Console` 10.0.0

2. **Refatorar `Function.cs`** para usar construtor primário que inicializa o DI:

   ```csharp
   public class Function
   {
       private readonly IServiceProvider _serviceProvider;

       public Function()
       {
           var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();

           var services = new ServiceCollection();
           services.AddSingleton<IConfiguration>(config);
           services.AddLogging(logging =>
           {
               logging.AddConsole();
               logging.SetMinimumLevel(LogLevel.Information);
           });

           // Registra Options e demais dependências via CrossCutting
           services.AddOrchestratorConfiguration(config);
           // (Subtasks das stories 03 e 04 adicionarão mais registros aqui)

           _serviceProvider = services.BuildServiceProvider(validateScopes: true);
       }

       public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
       {
           // A implementar nas stories 03 e 04
           foreach (var record in sqsEvent.Records)
           {
               context.Logger.LogInformation("Record recebido: {MessageId}", record.MessageId);
           }
       }
   }
   ```

3. **Garantir que o `ServiceProvider` é criado uma única vez** (no construtor), não a cada invocação do handler — este padrão é crítico para performance e correto reuso de clientes HTTP em Lambda.

## Formas de Teste

1. Teste unitário: verificar que `Function()` instancia sem exceção quando todas as variáveis de ambiente obrigatórias estão presentes (usando `Environment.SetEnvironmentVariable` no arrange).
2. Teste unitário: verificar que `Function()` lança `OptionsValidationException` quando uma variável obrigatória está ausente (comportamento do `ValidateOnStart`).
3. Execução local via Lambda Test Tool: confirmar que o handler loga o `MessageId` ao receber um evento SQS de teste sem erros de resolução de DI.

## Critérios de Aceite
- [ ] `Function.cs` inicializa `IConfiguration` via `AddEnvironmentVariables()` e constrói o `ServiceProvider` no construtor — não no handler.
- [ ] O `ServiceProvider` resolve `IOptions<VideoManagementApiOptions>`, `IOptions<M2MAuthOptions>` e `IOptions<StepFunctionOptions>` sem erros quando as variáveis de ambiente correspondentes estão presentes.
- [ ] `dotnet build` e `dotnet test` passam sem erros após as alterações.

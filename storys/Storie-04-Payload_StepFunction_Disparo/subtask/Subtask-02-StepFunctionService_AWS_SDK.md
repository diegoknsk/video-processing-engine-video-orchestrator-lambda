# Subtask-02: Implementar StepFunctionService com AWS SDK

## Descrição
Implementar o serviço responsável por disparar a execução da Step Function via `AWSSDK.StepFunctions`, lendo o ARN de `IOptions<StepFunctionOptions>` e logando o `executionArn` retornado pela AWS.

## Passos de Implementação

1. **Adicionar os pacotes NuGet** ao `.csproj` do projeto `Infra.Data`:
   - `AWSSDK.StepFunctions` 3.7.x (verificar versão mais recente estável)
   - `AWSSDK.Extensions.NETCore.Setup` 3.7.x (para registro facilitado no DI)

2. **Criar a interface de porta** `IStepFunctionService` em `Application/Ports/`:

   ```csharp
   public interface IStepFunctionService
   {
       Task<string> StartExecutionAsync(
           StepFunctionPayload payload,
           CancellationToken ct = default);
   }
   ```
   Retorna o `executionArn` gerado pela AWS.

3. **Implementar `StepFunctionService`** em `Infra.Data/AwsServices/`:

   ```csharp
   public class StepFunctionService(
       IAmazonStepFunctions stepFunctions,
       IOptions<StepFunctionOptions> options,
       ILogger<StepFunctionService> logger) : IStepFunctionService
   {
       public async Task<string> StartExecutionAsync(StepFunctionPayload payload, CancellationToken ct = default)
       {
           var stateMachineArn = options.Value.StateMachineArn;
           var inputJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions
           {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
           });

           logger.LogInformation(
               "Starting Step Function execution for VideoId={VideoId}, ExecutionId={ExecutionId}",
               payload.Video.VideoId, payload.ExecutionId);

           var request = new StartExecutionRequest
           {
               StateMachineArn = stateMachineArn,
               Name = payload.ExecutionId,
               Input = inputJson
           };

           try
           {
               var response = await stepFunctions.StartExecutionAsync(request, ct);
               logger.LogInformation(
                   "Step Function execution started: ExecutionArn={ExecutionArn}",
                   response.ExecutionArn);
               return response.ExecutionArn;
           }
           catch (Exception ex)
           {
               logger.LogError(ex,
                   "Failed to start Step Function execution for VideoId={VideoId}",
                   payload.Video.VideoId);
               throw;
           }
       }
   }
   ```

4. **Registrar no DI** em `DependencyInjection.cs`:

   ```csharp
   // Registrar o cliente AWS Step Functions (usa credenciais IAM da role da Lambda)
   services.AddAWSService<IAmazonStepFunctions>();

   services.AddScoped<IStepFunctionService, StepFunctionService>();
   ```

   > `AddAWSService<T>` requer `AWSSDK.Extensions.NETCore.Setup`. Ele resolve a região a partir de `AWS_REGION` (variável de ambiente já presente no ambiente Lambda).

## Formas de Teste

1. Teste unitário com mock de `IAmazonStepFunctions`: `StartExecutionAsync` retorna `ExecutionArn` esperado quando o SDK responde com sucesso.
2. Teste unitário: SDK lança `AmazonStepFunctionsException` → `StepFunctionService` relança a exceção após logar o erro.
3. Teste unitário: verificar que o input JSON enviado ao SDK contém `videoId`, `userId`, `user.email`, `user.name`, `video.chunks` (array com ao menos 1 item) e `zip.outputKey` — via captura e deserialização do argumento passado ao mock de `IAmazonStepFunctions`.

## Critérios de Aceite
- [ ] `StepFunctionService.StartExecutionAsync` chama `IAmazonStepFunctions.StartExecutionAsync` com o ARN lido de `IOptions<StepFunctionOptions>` e o payload serializado como JSON camelCase.
- [ ] O `executionArn` retornado pela AWS é logado como `Information` após o disparo bem-sucedido.
- [ ] Exceção do SDK é capturada, logada como `Error` com `VideoId` no placeholder, e relançada — sem swallow.

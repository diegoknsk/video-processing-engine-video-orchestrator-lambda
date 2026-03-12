# Subtask-03: Implementar OrchestrateVideoProcessingUseCase e integrar no handler

## Descrição
Implementar o `OrchestrateVideoProcessingUseCase` que recebe o `VideoDetails` (da Story 03), monta o payload completo e dispara a Step Function. Atualizar o `FunctionHandler` para chamar este UseCase após o `FetchVideoDetailsUseCase`, completando o fluxo ponta a ponta sem comportamento mockado.

## Passos de Implementação

1. **Criar `IOrchestrateVideoProcessingUseCase`** em `Application/UseCases/`:

   ```csharp
   public interface IOrchestrateVideoProcessingUseCase
   {
       Task<string> ExecuteAsync(VideoDetails videoDetails, CancellationToken ct = default);
   }
   ```
   Retorna o `executionArn` da Step Function disparada.

2. **Implementar `OrchestrateVideoProcessingUseCase`** em `Application/UseCases/OrchestrateVideoProcessing/`:

   ```csharp
   public class OrchestrateVideoProcessingUseCase(
       IStepFunctionService stepFunctionService,
       ILogger<OrchestrateVideoProcessingUseCase> logger) : IOrchestrateVideoProcessingUseCase
   {
       public async Task<string> ExecuteAsync(VideoDetails videoDetails, CancellationToken ct = default)
       {
           logger.LogInformation(
               "Building Step Function payload for VideoId={VideoId}, UserId={UserId}",
               videoDetails.VideoId, videoDetails.UserId);

           var executionId = $"exec-{videoDetails.VideoId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
           var payload = StepFunctionPayloadBuilder.Build(videoDetails, executionId);

           logger.LogInformation(
               "Dispatching Step Function for VideoId={VideoId}, ExecutionId={ExecutionId}",
               videoDetails.VideoId, executionId);

           var executionArn = await stepFunctionService.StartExecutionAsync(payload, ct);

           logger.LogInformation(
               "Step Function dispatched successfully. VideoId={VideoId}, ExecutionArn={ExecutionArn}",
               videoDetails.VideoId, executionArn);

           return executionArn;
       }
   }
   ```

3. **Atualizar `Function.cs`** para compor os dois UseCases no handler:

   ```csharp
   public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
   {
       using var scope = _serviceProvider.CreateScope();
       var fetchUseCase = scope.ServiceProvider.GetRequiredService<IFetchVideoDetailsUseCase>();
       var orchestrateUseCase = scope.ServiceProvider.GetRequiredService<IOrchestrateVideoProcessingUseCase>();

       foreach (var record in sqsEvent.Records)
       {
           context.Logger.LogInformation("Processing record {MessageId}", record.MessageId);

           var s3Event = JsonSerializer.Deserialize<S3ObjectCreatedEvent>(record.Body)
               ?? throw new InvalidOperationException($"Cannot deserialize S3 event from record {record.MessageId}");

           foreach (var s3Record in s3Event.Records)
           {
               var videoDetails = await fetchUseCase.ExecuteAsync(s3Record.S3.Object.Key);
               var executionArn = await orchestrateUseCase.ExecuteAsync(videoDetails);

               context.Logger.LogInformation(
                   "Orchestration complete for VideoId={VideoId}. ExecutionArn={ExecutionArn}",
                   videoDetails.VideoId, executionArn);
           }
       }
   }
   ```

4. **Registrar no DI**:
   ```csharp
   services.AddScoped<IOrchestrateVideoProcessingUseCase, OrchestrateVideoProcessingUseCase>();
   ```

5. **Remover qualquer lógica mockada** remanescente no `Function.cs` (ex.: o `Console.WriteLine` original).

## Formas de Teste

1. Teste unitário: `OrchestrateVideoProcessingUseCase.ExecuteAsync` com mock de `IStepFunctionService` → verifica que `StartExecutionAsync` é chamado e `executionArn` é retornado corretamente.
2. Teste unitário: `IStepFunctionService` lança exceção → UseCase propaga sem swallow.
3. Teste de integração local (Lambda Test Tool): enviar o evento S3 real do projeto (conforme exemplo em `docs/sampleSqsOrquestrador.txt` ou evento direto do S3) e verificar nos logs que o fluxo completo é executado — parse → token → busca → payload → disparo (com as dependências externas mockadas ou reais em ambiente dev).

## Critérios de Aceite
- [ ] `OrchestrateVideoProcessingUseCase.ExecuteAsync` monta o payload via `StepFunctionPayloadBuilder` — o payload passado ao `IStepFunctionService` contém `video.chunks` populado e `zip` com `outputBucket` e `outputKey`.
- [ ] O `FunctionHandler` não contém mais nenhuma lógica mockada — o fluxo completo passa por `FetchVideoDetailsUseCase` → `OrchestrateVideoProcessingUseCase`.
- [ ] O `executionArn` é logado ao final de cada evento processado com sucesso.

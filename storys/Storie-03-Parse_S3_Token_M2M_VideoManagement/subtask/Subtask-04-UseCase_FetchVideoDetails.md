# Subtask-04: Implementar FetchVideoDetailsUseCase e integrar no handler

## Descrição
Implementar o `FetchVideoDetailsUseCase` que orquestra as etapas: parse da key S3 → obtenção do token M2M → busca de detalhes do vídeo. Integrar o UseCase no `FunctionHandler` do Lambda para que a execução real substitua o comportamento mockado.

## Passos de Implementação

1. **Criar `IFetchVideoDetailsUseCase`** em `Application/UseCases/`:

   ```csharp
   public interface IFetchVideoDetailsUseCase
   {
       Task<VideoDetails> ExecuteAsync(string s3Key, CancellationToken ct = default);
   }
   ```

2. **Implementar `FetchVideoDetailsUseCase`** em `Application/UseCases/FetchVideoDetails/`:

   ```csharp
   public class FetchVideoDetailsUseCase(
       IM2MTokenService tokenService,
       IVideoManagementClient videoManagementClient,
       ILogger<FetchVideoDetailsUseCase> logger) : IFetchVideoDetailsUseCase
   {
       public async Task<VideoDetails> ExecuteAsync(string s3Key, CancellationToken ct = default)
       {
           logger.LogInformation("Parsing S3 key {S3Key}", s3Key);
           var (userId, videoId) = S3KeyParser.Parse(s3Key);
           logger.LogInformation("Extracted UserId={UserId}, VideoId={VideoId}", userId, videoId);

           logger.LogInformation("Requesting M2M token for UserId={UserId}", userId);
           var token = await tokenService.GetAccessTokenAsync(ct);

           logger.LogInformation("Fetching video details for UserId={UserId}, VideoId={VideoId}", userId, videoId);
           var details = await videoManagementClient.GetVideoDetailsAsync(userId, videoId, token, ct);
           logger.LogInformation("Video details fetched successfully for VideoId={VideoId}", videoId);

           return details;
       }
   }
   ```

3. **Atualizar `Function.cs`** para registrar o UseCase no DI e chamá-lo no handler.

   > **Detalhe de interface layer — não é contrato funcional desta story.** O handler atual usa `SQSEvent` porque o trigger configurado no ambiente é SQS. Isso é um detalhe de entrega do evento, confinado ao `Function.cs`. O contrato funcional desta story começa após a extração da `s3Key` — os UseCases não sabem nem precisam saber como o evento chegou.

   Referência de como o `Function.cs` (interface layer) extrai a key e delega ao UseCase com o transporte atual:

   ```csharp
   // Trecho exclusivo do Function.cs — interface layer
   // Responsável por unwrap do envelope de transporte e extração da s3Key
   foreach (var record in sqsEvent.Records)
   {
       var s3Event = JsonSerializer.Deserialize<S3ObjectCreatedEvent>(record.Body)
           ?? throw new InvalidOperationException("S3 event payload inválido");

       foreach (var s3Record in s3Event.Records)
       {
           var s3Key = s3Record.S3.Object.Key;
           // A partir daqui, apenas a s3Key é passada — sem acoplamento ao SQS
           var videoDetails = await useCase.ExecuteAsync(s3Key);
       }
   }
   ```

4. **Registrar `FetchVideoDetailsUseCase` no DI** em `DependencyInjection.cs`:
   ```csharp
   services.AddScoped<IFetchVideoDetailsUseCase, FetchVideoDetailsUseCase>();
   ```

## Formas de Teste

1. Teste unitário de `FetchVideoDetailsUseCase.ExecuteAsync` com mocks de `IM2MTokenService` e `IVideoManagementClient`: cenário feliz → retorna `VideoDetails` com userId/videoId corretos.
2. Teste unitário: `S3KeyParser` lança `FormatException` → UseCase propaga exceção sem swallow.
3. Teste unitário: `IM2MTokenService` lança exceção de domínio → UseCase propaga sem conversão dupla.

## Critérios de Aceite
- [ ] `FetchVideoDetailsUseCase.ExecuteAsync` retorna `VideoDetails` quando todas as dependências respondem com sucesso.
- [ ] `FetchVideoDetailsUseCase` não contém nenhuma referência a `SQSEvent`, `SQSMessage` ou qualquer tipo de envelope de transporte — opera exclusivamente sobre `s3Key` como string.
- [ ] Exceções de `S3KeyParser`, `IM2MTokenService` e `IVideoManagementClient` propagam corretamente sem swallow; o comportamento de reprocessamento é responsabilidade da infraestrutura de mensageria, não do UseCase.
- [ ] Logs estruturados presentes em todos os pontos: parse, token, busca — com `UserId` e `VideoId` nos placeholders.

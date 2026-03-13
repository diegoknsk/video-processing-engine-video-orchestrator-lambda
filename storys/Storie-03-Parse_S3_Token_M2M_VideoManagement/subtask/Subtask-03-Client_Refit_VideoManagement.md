# Subtask-03: Implementar client Refit para API interna Video Management

## Descrição
Implementar o client Refit para consumir o endpoint `GET /internal/videos/{userId}/{videoId}` da API de Video Management, com autenticação Bearer, resilience e mapeamento da resposta para o modelo de domínio `VideoDetails`.

## Passos de Implementação

1. **Confirmar com o time de Video Management o contrato de resposta** do endpoint `GET /internal/videos/{userId}/{videoId}`. A resposta deve conter dados do vídeo **e** dados do usuário (nome, email) — conforme decisão arquitetural: o orquestrador não acessa Cognito. Documentar os campos esperados antes de criar os contracts.

2. **Criar a interface Refit `IVideoManagementApi`** em `Infra.Data/ExternalApis/VideoManagement/`:

   ```csharp
   public interface IVideoManagementApi
   {
       [Get("/internal/videos/{userId}/{videoId}")]
       Task<VideoManagementVideoResponse> GetVideoDetailsAsync(
           string userId,
           string videoId,
           [Header("Authorization")] string bearerToken,
           CancellationToken ct = default);
   }
   ```

3. **Criar o contrato de resposta** `VideoManagementVideoResponse` (record) em `Infra.Data/ExternalApis/VideoManagement/Contracts/` com os campos retornados pela API (confirmar com o contrato real — exemplo mínimo):

   ```csharp
   public record VideoManagementVideoResponse(
       [property: JsonPropertyName("videoId")] string VideoId,
       [property: JsonPropertyName("userId")] string UserId,
       [property: JsonPropertyName("title")] string Title,
       [property: JsonPropertyName("status")] string Status,
       [property: JsonPropertyName("s3Key")] string S3Key,
       [property: JsonPropertyName("user")] VideoManagementUserInfo User
   );

   public record VideoManagementUserInfo(
       [property: JsonPropertyName("name")] string Name,
       [property: JsonPropertyName("email")] string Email
   );
   ```

4. **Criar a interface de porta** `IVideoManagementClient` em `Application/Ports/`:

   ```csharp
   public interface IVideoManagementClient
   {
       Task<VideoDetails> GetVideoDetailsAsync(string userId, string videoId, string accessToken, CancellationToken ct = default);
   }
   ```

5. **Criar o modelo de domínio `VideoDetails`** em `Domain/Models/` com os campos necessários para a Story 04 (dados do vídeo + dados do usuário). Este modelo não tem dependência de Refit.

6. **Criar `VideoManagementClientService`** em `Infra.Data/ExternalApis/VideoManagement/` implementando `IVideoManagementClient`:
   - Injetar `IVideoManagementApi`, `ILogger<VideoManagementClientService>`.
   - Formatar o token como `"Bearer {accessToken}"` ao passar para Refit.
   - Mapear `ApiException` 404 para `NotFoundException` de domínio; outros 4xx/5xx para `ExternalServiceException`.
   - Mapear `VideoManagementVideoResponse` para `VideoDetails`.

7. **Registrar no DI**:
   ```csharp
   services.AddRefitClient<IVideoManagementApi>()
       .ConfigureHttpClient((sp, client) =>
       {
           var opts = sp.GetRequiredService<IOptions<VideoManagementApiOptions>>().Value;
           client.BaseAddress = new Uri(opts.BaseUrl);
           client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
       })
       .AddStandardResilienceHandler(o =>
       {
           o.Retry.MaxRetryAttempts = 3;
           o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
           o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
       });

   services.AddScoped<IVideoManagementClient, VideoManagementClientService>();
   ```

## Formas de Teste

1. Teste unitário com mock de `IVideoManagementApi`: resposta de sucesso → `GetVideoDetailsAsync` retorna `VideoDetails` com campos mapeados corretamente.
2. Teste unitário: `ApiException` 404 → `VideoManagementClientService` lança `NotFoundException` (não `ApiException`).
3. Teste unitário: verificar que o token é enviado como `"Bearer {token}"` — verificar o argumento passado ao mock de `IVideoManagementApi`.

## Critérios de Aceite
- [ ] `VideoManagementClientService.GetVideoDetailsAsync` retorna `VideoDetails` com dados de vídeo e usuário quando a API responde com sucesso.
- [ ] HTTP 404 da API lança `NotFoundException` de domínio (sem expor `ApiException` ao UseCase).
- [ ] O token é sempre enviado no formato `Authorization: Bearer {token}` — verificado por teste unitário.

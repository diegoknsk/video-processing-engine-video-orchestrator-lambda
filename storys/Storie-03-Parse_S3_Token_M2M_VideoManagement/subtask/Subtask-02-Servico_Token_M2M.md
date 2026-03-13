# Subtask-02: Implementar serviço de token M2M (client credentials)

## Descrição
Implementar o serviço responsável por obter o token de acesso interno via OAuth2 client credentials grant. O serviço deve usar Refit, ler credenciais de `IOptions<M2MAuthOptions>` e nunca logar o valor do secret ou do token retornado.

## Passos de Implementação

1. **Criar a interface Refit `IM2MAuthApi`** em `Infra.Data/ExternalApis/M2MAuth/`:

   ```csharp
   // Infra.Data/ExternalApis/M2MAuth/IM2MAuthApi.cs
   public interface IM2MAuthApi
   {
       [Post("")]  // URL base é o TokenEndpoint completo via BaseAddress
       [Headers("Content-Type: application/x-www-form-urlencoded")]
       Task<M2MTokenResponse> GetTokenAsync(
           [Body(BodySerializationMethod.UrlEncoded)] M2MTokenRequest request,
           CancellationToken ct = default);
   }
   ```

2. **Criar os contratos** (records) em `Infra.Data/ExternalApis/M2MAuth/Contracts/`:

   ```csharp
   public record M2MTokenRequest(
       [property: AliasAs("grant_type")] string GrantType,
       [property: AliasAs("client_id")] string ClientId,
       [property: AliasAs("client_secret")] string ClientSecret
   );

   public record M2MTokenResponse(
       [property: JsonPropertyName("access_token")] string AccessToken,
       [property: JsonPropertyName("expires_in")] int ExpiresIn,
       [property: JsonPropertyName("token_type")] string TokenType
   );
   ```

3. **Criar a interface de porta** em `Application/Ports/IM2MTokenService.cs`:

   ```csharp
   public interface IM2MTokenService
   {
       Task<string> GetAccessTokenAsync(CancellationToken ct = default);
   }
   ```

4. **Criar o `M2MTokenService`** em `Infra.Data/ExternalApis/M2MAuth/M2MTokenService.cs`:
   - Injetar `IM2MAuthApi` e `IOptions<M2MAuthOptions>` e `ILogger<M2MTokenService>`.
   - Logar início da solicitação de token (sem logar `ClientSecret`).
   - Capturar `ApiException`, mapear para exceção de domínio com log de erro (status code apenas).
   - Retornar `AccessToken` em caso de sucesso.

5. **Registrar no DI** via `DependencyInjection.cs` (CrossCutting):

   ```csharp
   services.AddRefitClient<IM2MAuthApi>()
       .ConfigureHttpClient((sp, client) =>
       {
           var opts = sp.GetRequiredService<IOptions<M2MAuthOptions>>().Value;
           client.BaseAddress = new Uri(opts.TokenEndpoint);
       })
       .AddStandardResilienceHandler(o =>
       {
           o.Retry.MaxRetryAttempts = 2;
           o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
       });

   services.AddScoped<IM2MTokenService, M2MTokenService>();
   ```

## Formas de Teste

1. Teste unitário com mock de `IM2MAuthApi`: cenário de sucesso → `GetAccessTokenAsync` retorna o `access_token` da resposta.
2. Teste unitário: `IM2MAuthApi` lança `ApiException` com status 401 → `M2MTokenService` relança exceção de domínio (não `ApiException`).
3. Verificar via log mock (`ILogger`) que `ClientSecret` **não aparece** em nenhuma chamada de log.

## Critérios de Aceite
- [ ] `M2MTokenService.GetAccessTokenAsync` retorna o token quando `IM2MAuthApi` responde com sucesso.
- [ ] Nenhuma linha de código loga o valor de `ClientSecret` ou do token retornado.
- [ ] `ApiException` é capturada e convertida para exceção de domínio antes de propagar ao UseCase.

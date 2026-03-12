# Subtask-05: Testes unitários — Story 03

## Descrição
Criar e consolidar todos os testes unitários dos componentes da Story 03: `S3KeyParser`, `M2MTokenService`, `VideoManagementClientService` e `FetchVideoDetailsUseCase`. Garantir cobertura ≥ 80% nos novos componentes.

## Passos de Implementação

1. **Criar testes para `S3KeyParser`** (`S3KeyParserTests.cs`):
   - `Parse_ValidKey_ReturnsUserIdAndVideoId` — key `videos/user-123/video-456/original` → `("user-123", "video-456")`.
   - `Parse_KeyWithoutExpectedPrefix_ThrowsFormatException` — key `uploads/user/video/original`.
   - `Parse_KeyWithoutExpectedSuffix_ThrowsFormatException` — key `videos/user/video/raw`.
   - `Parse_KeyWithTooManySegments_ThrowsFormatException` — key `videos/user/sub/video/original`.
   - `Parse_NullOrEmptyKey_ThrowsArgumentException`.
   - `Parse_RealS3KeyFromPromptSample_ReturnsCorrectIds` — key do evento de exemplo do projeto.

2. **Criar testes para `M2MTokenService`** (`M2MTokenServiceTests.cs`):
   - `GetAccessTokenAsync_WhenApiReturnsToken_ReturnsAccessToken`.
   - `GetAccessTokenAsync_When401_ThrowsDomainException` (não `ApiException`).
   - `GetAccessTokenAsync_WhenApiThrowsGenericError_ThrowsDomainException`.
   - Verificar via mock de `ILogger` que nenhum log contém a string do ClientSecret.

3. **Criar testes para `VideoManagementClientService`** (`VideoManagementClientServiceTests.cs`):
   - `GetVideoDetailsAsync_WhenApiReturnsVideo_MapsToVideoDetails` — verifica campos de vídeo e usuário.
   - `GetVideoDetailsAsync_When404_ThrowsNotFoundException`.
   - `GetVideoDetailsAsync_WhenBearerTokenFormatted_PassesCorrectHeader` — verificar argumento do mock.

4. **Criar testes para `FetchVideoDetailsUseCase`** (`FetchVideoDetailsUseCaseTests.cs`):
   - `ExecuteAsync_HappyPath_ReturnsMappedVideoDetails`.
   - `ExecuteAsync_InvalidS3Key_PropagatesFormatException`.
   - `ExecuteAsync_TokenServiceFails_PropagatesException`.
   - `ExecuteAsync_VideoManagementFails_PropagatesException`.

5. **Executar cobertura**:
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
   ```
   Verificar relatório e garantir ≥ 80% nos arquivos da Story 03.

## Formas de Teste

1. `dotnet test` — todos os testes passam sem erros.
2. Relatório de coverlet — cobertura ≥ 80% nos componentes: `S3KeyParser`, `M2MTokenService`, `VideoManagementClientService`, `FetchVideoDetailsUseCase`.
3. Revisão manual: confirmar que os cenários de erro (parse inválido, 404, 401) estão todos cobertos com asserções específicas (não apenas verificação de que não lança exceção).

## Critérios de Aceite
- [ ] Todos os testes listados nos passos 1–4 estão implementados e passam com `dotnet test`.
- [ ] Cobertura ≥ 80% nos 4 componentes principais da Story 03.
- [ ] Nenhum teste usa `Thread.Sleep` ou depende de estado compartilhado entre testes — todos são independentes e deterministicos.

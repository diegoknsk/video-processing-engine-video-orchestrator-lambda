# Subtask-04: Testes unitários — Story 04

## Descrição
Criar e consolidar todos os testes unitários dos componentes da Story 04: `StepFunctionPayloadBuilder`, `StepFunctionService` e `OrchestrateVideoProcessingUseCase`. Garantir cobertura ≥ 80% nos novos componentes.

## Passos de Implementação

1. **Criar testes para `StepFunctionPayloadBuilder`** (`StepFunctionPayloadBuilderTests.cs`):
   - `Build_WithValidVideoDetails_ReturnsPayloadWithCorrectVideoId`.
   - `Build_WithValidVideoDetails_DerivesOutputPrefixCorrectly` — verifica `videos/{userId}/{videoId}/frames/`.
   - `Build_WithValidVideoDetails_DerivesZipOutputBucketCorrectly` — verifica que `zip.outputBucket` é igual ao `s3Bucket` do vídeo de entrada.
   - `Build_WithValidVideoDetails_DerivesZipOutputKeyCorrectly` — verifica `videos/{userId}/{videoId}/output.zip` com asserção de string exata.
   - `Build_WithValidVideoDetails_ChunksContainsAtLeastOneItem` — verifica que `chunks` não é vazio.
   - `Build_WithValidVideoDetails_FirstChunkHasCorrectOutputPath` — verifica `outputPath` do chunk 0 como `videos/{userId}/{videoId}/frames/chunk-0/`.
   - `Build_WithValidVideoDetails_MapsUserNameAndEmail`.
   - `Build_PayloadSerializesToValidJson` — serializa com `JsonNamingPolicy.CamelCase` e verifica que o JSON contém as chaves: `executionId`, `video.chunks`, `zip.outputBucket`, `zip.outputKey`, `user.name`, `user.email`.

2. **Criar testes para `StepFunctionService`** (`StepFunctionServiceTests.cs`):
   - `StartExecutionAsync_WhenSdkSucceeds_ReturnsExecutionArn`.
   - `StartExecutionAsync_WhenSdkThrows_PropagatesException`.
   - `StartExecutionAsync_VerifyInputJsonContainsVideoId` — capturar argumento do mock `IAmazonStepFunctions` e verificar JSON.
   - `StartExecutionAsync_VerifyStateMachineArnFromOptions` — verificar que o ARN passado ao SDK vem de `IOptions`.

3. **Criar testes para `OrchestrateVideoProcessingUseCase`** (`OrchestrateVideoProcessingUseCaseTests.cs`):
   - `ExecuteAsync_HappyPath_CallsStepFunctionServiceAndReturnsExecutionArn`.
   - `ExecuteAsync_WhenStepFunctionFails_PropagatesException`.
   - `ExecuteAsync_VerifyExecutionIdContainsVideoId` — verificar que o `ExecutionId` no payload passado ao mock contém o `VideoId`.

4. **Executar cobertura**:
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
   ```
   Verificar relatório e garantir ≥ 80% nos arquivos da Story 04.

5. **Verificar `dotnet build` e `dotnet test` no pipeline** — confirmar que todas as stories (02, 03, 04) passam juntas sem conflito.

## Formas de Teste

1. `dotnet test` — todos os testes das 3 stories passam sem erros.
2. Relatório de coverlet — cobertura ≥ 80% nos componentes: `StepFunctionPayloadBuilder`, `StepFunctionService`, `OrchestrateVideoProcessingUseCase`.
3. Revisão manual: confirmar que todos os cenários de erro (SDK falha, payload inválido) têm testes explícitos com asserções de exceção.

## Critérios de Aceite
- [ ] Todos os testes listados nos passos 1–3 estão implementados e passam com `dotnet test`.
- [ ] Cobertura ≥ 80% nos 3 componentes principais da Story 04.
- [ ] `dotnet build` e `dotnet test` passam no pipeline do GitHub Actions com a suite completa das stories 02, 03 e 04.

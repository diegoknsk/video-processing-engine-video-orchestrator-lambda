# Subtask-05: Testes unitários — atualizar e criar cobertura do novo contrato

## Descrição
Atualizar os testes unitários existentes que referenciavam o modelo antigo de `VideoChunk` (com `OutputPath`) e criar novos testes cobrindo: montagem do payload com defaults (sem variáveis de ambiente), montagem com opções customizadas, verificação de todos os campos obrigatórios nos blocos `output`, `finalize` e em cada chunk, e garantir que `dotnet test` passa com cobertura ≥ 80% nos componentes alterados.

## Passos de Implementação

1. **Atualizar testes existentes** que referenciam `VideoChunk.OutputPath` ou campos removidos — substituir pelas asserções nos novos campos (`FramesPrefix`, `ChunkId`, etc.); atualizar asserções do `ZipOutputInfo.OutputBucket` para refletir que o bucket agora vem de `OutputOptions.ZipBucket`.

2. **Criar testes no `StepFunctionPayloadBuilder`** (ou classe de testes dedicada):
   - `Build_WithDefaultOptions_ReturnsPayloadWithDefaultBuckets`: passa `new OutputOptions()` e verifica que `output.FramesBucket`, `output.ManifestBucket`, `zip.OutputBucket` e `finalize.OutputBucket` têm os valores default.
   - `Build_WithCustomOptions_ReturnsPayloadWithCustomBuckets`: passa `OutputOptions` com buckets customizados e verifica todos os blocos.
   - `Build_Chunk0_HasCorrectFields`: verifica `chunkIndex == 0`, `chunkId == "{videoId}-chunk-0"`, `startSec == 0`, `endSec == -1`, `intervalSec == options.FrameIntervalSec`, `manifestPrefix` e `framesPrefix` com paths corretos.
   - `Build_FinalizeBlock_ContainsAllRequiredFields`: verifica todos os campos de `FinalizeInfo` incluindo `framesBasePrefix` (path base, não por chunk), `outputBasePrefix`, `videoId` e `ordenaAutomaticamente`.
   - `Build_ContractVersion_Is_1_0`: verifica que `contractVersion == "1.0"`.

3. **Criar/atualizar testes do `OrchestrateVideoProcessingUseCase`**:
   - Verificar que o UseCase passa o `OutputOptions.Value` correto ao builder (indireto — verificar o payload recebido pelo mock de `IStepFunctionService`).
   - Verificar que log de `ExecutionId` e `ExecutionArn` ainda ocorre após a mudança.

4. **Executar `dotnet test` com cobertura** (se configurado no projeto) e verificar que os componentes novos/alterados atingem ≥ 80%.

## Formas de Teste

1. **`dotnet test`** — deve passar 100% sem falhas ou erros de compilação.
2. **Verificação manual dos asserts:** cada teste cobre um campo específico do payload; nenhum teste verifica "qualquer coisa" de forma genérica — cada assert é específico e mensurável.
3. **Revisão de nomes de teste:** nomes seguem o padrão `Método_Cenário_ResultadoEsperado` para facilitar leitura no output do test runner.

## Critérios de Aceite

- [ ] Todos os testes que referenciavam `OutputPath` em `VideoChunk` foram atualizados para usar `FramesPrefix`.
- [ ] Novos testes cobrem os 5 cenários descritos nos passos de implementação (defaults, customizados, chunk fields, finalize, contractVersion).
- [ ] `dotnet test` passa com zero falhas após as alterações desta e das subtasks anteriores.
- [ ] Cobertura ≥ 80% nos componentes `StepFunctionPayloadBuilder` e `OrchestrateVideoProcessingUseCase`.
- [ ] Nenhum teste usa valor hardcoded de bucket de produção — todos usam `OutputOptions` construído explicitamente no arrange do teste.

# Subtask-03: Refatorar StepFunctionPayloadBuilder com novo contrato

## Descrição
Refatorar `StepFunctionPayloadBuilder` para montar o payload completo segundo o novo contrato, computando os campos novos de cada chunk (`chunkId`, `startSec`, `endSec`, `intervalSec`, `manifestPrefix`, `framesPrefix`), montando o bloco `output` e o bloco `finalize` a partir de `OutputOptions`. Todos os valores de bucket devem vir do `OutputOptions` — nenhum bucket hardcoded em código de produção.

## Passos de Implementação

1. **Alterar a assinatura do método `Build`** para receber `OutputOptions` como parâmetro adicional:
   ```csharp
   public static StepFunctionPayload Build(
       VideoDetails details,
       string executionId,
       OutputOptions outputOptions)
   ```

2. **Montar `VideoChunk` com os novos campos**:
   - `ChunkIndex`: `0` (chunk único — toda a duração do vídeo)
   - `ChunkId`: `$"{details.VideoId}-chunk-0"`
   - `StartSec`: `0`
   - `EndSec`: `-1` (convenção: até o fim do vídeo; processador interpreta como duração total)
   - `IntervalSec`: `outputOptions.FrameIntervalSec`
   - `ManifestPrefix`: `$"videos/{details.UserId}/{details.VideoId}/manifests/chunk-0/"`
   - `FramesPrefix`: `$"videos/{details.UserId}/{details.VideoId}/frames/chunk-0/"`

3. **Montar bloco `Output`**:
   ```csharp
   new OutputInfo(
       FramesBucket: outputOptions.FramesBucket,
       ManifestBucket: outputOptions.ManifestBucket
   )
   ```

4. **Montar bloco `Zip`** — atualizar `OutputBucket` para usar `outputOptions.ZipBucket` (em vez de `details.S3Bucket`):
   ```csharp
   new ZipOutputInfo(
       OutputBucket: outputOptions.ZipBucket,
       OutputKey: $"videos/{details.UserId}/{details.VideoId}/output.zip"
   )
   ```

5. **Montar bloco `Finalize`**:
   ```csharp
   new FinalizeInfo(
       FramesBucket: outputOptions.FramesBucket,
       FramesBasePrefix: $"videos/{details.UserId}/{details.VideoId}/frames/",
       OutputBucket: outputOptions.ZipBucket,
       VideoId: details.VideoId,
       OutputBasePrefix: $"{details.UserId}/{details.VideoId}",
       OrdenaAutomaticamente: outputOptions.OrdenaAutomaticamente
   )
   ```

6. **Adicionar `ContractVersion`** ao `StepFunctionPayload`:
   - Valor fixo `"1.0"` definido como constante privada no builder (ex.: `private const string ContractVersion = "1.0";`).
   - Não é um valor de configuração; representa a versão do schema do payload.

## Formas de Teste

1. **Teste unitário — payload com defaults:** chamar `Build` com `OutputOptions` criado com os valores default; verificar que todos os campos do payload têm os valores esperados calculados a partir dos defaults.
2. **Teste unitário — payload com opções customizadas:** passar `OutputOptions` com buckets diferentes dos defaults e verificar que os blocos `output`, `zip` e `finalize` refletem os valores customizados.
3. **Teste de campos de chunk:** verificar que `chunks[0]` contém `ChunkId == "{videoId}-chunk-0"`, `StartSec == 0`, `EndSec == -1`, `IntervalSec == outputOptions.FrameIntervalSec`, `ManifestPrefix` e `FramesPrefix` com os paths corretos.

## Critérios de Aceite

- [ ] `StepFunctionPayloadBuilder.Build` recebe `OutputOptions` e monta todos os blocos do novo contrato corretamente.
- [ ] Nenhum nome de bucket está hardcoded no método `Build`; todos vêm de `outputOptions`.
- [ ] `ContractVersion` é `"1.0"` e está presente no payload retornado.
- [ ] `Finalize.FramesBasePrefix` aponta para o path base de frames (`videos/{userId}/{videoId}/frames/`), não para um chunk específico.
- [ ] `Zip.OutputBucket` usa `outputOptions.ZipBucket` (não mais `details.S3Bucket`).
- [ ] Testes unitários passam com defaults e com valores customizados.

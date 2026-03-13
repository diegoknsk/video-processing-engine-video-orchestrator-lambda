# Subtask-02: Atualizar modelos de domínio — VideoChunk, OutputInfo, FinalizeInfo, StepFunctionPayload

## Descrição
Atualizar o arquivo `StepFunctionPayload.cs` no projeto `Domain` para refletir o novo contrato esperado pela Step Function: adicionar `ContractVersion` ao `StepFunctionPayload`, substituir os campos de `VideoChunk` (removendo `OutputPath`, adicionando `ChunkId`, `StartSec`, `EndSec`, `IntervalSec`, `ManifestPrefix` e `FramesPrefix`), criar o record `OutputInfo` (com `FramesBucket` e `ManifestBucket`) e criar o record `FinalizeInfo` (com todos os dados necessários para o lambda de finalize).

## Passos de Implementação

1. **Atualizar `VideoChunk`** em `StepFunctionPayload.cs`:
   - Remover: `OutputPath`
   - Adicionar: `ChunkId` (string), `StartSec` (int), `EndSec` (int), `IntervalSec` (int), `ManifestPrefix` (string), `FramesPrefix` (string)
   - Manter: `ChunkIndex` (int)

2. **Criar record `OutputInfo`** no mesmo arquivo:
   ```csharp
   public sealed record OutputInfo(
       string FramesBucket,
       string ManifestBucket
   );
   ```

3. **Criar record `FinalizeInfo`** no mesmo arquivo:
   ```csharp
   public sealed record FinalizeInfo(
       string FramesBucket,
       string FramesBasePrefix,
       string OutputBucket,
       string VideoId,
       string OutputBasePrefix,
       bool OrdenaAutomaticamente
   );
   ```

4. **Atualizar `StepFunctionPayload`** (record raiz):
   - Adicionar propriedade `ContractVersion` (string) como primeiro campo.
   - Adicionar propriedade `Output` (`OutputInfo`) após `Video`.
   - Adicionar propriedade `Finalize` (`FinalizeInfo`) após `Zip`.
   - A ordem dos campos no record deve refletir a ordem de serialização desejada: `ContractVersion`, `ExecutionId`, `Video`, `Output`, `Zip`, `Finalize`, `User`.

## Formas de Teste

1. **Verificação de compilação:** `dotnet build` sem erros após as alterações no modelo — garante que todos os arquivos que referenciam `VideoChunk`, `StepFunctionPayload` e demais records compilam corretamente.
2. **Teste de serialização:** criar instâncias dos records com dados de teste e serializar com `System.Text.Json` (camelCase); verificar que o JSON gerado contém todos os campos esperados com os nomes corretos.
3. **Testes existentes:** executar `dotnet test` e corrigir qualquer falha de compilação ou asserção decorrente da remoção de `OutputPath` e da adição dos novos campos.

## Critérios de Aceite

- [ ] `VideoChunk` não contém mais `OutputPath`; contém `ChunkId`, `StartSec`, `EndSec`, `IntervalSec`, `ManifestPrefix` e `FramesPrefix`.
- [ ] `StepFunctionPayload` contém `ContractVersion`, `Output` (`OutputInfo`) e `Finalize` (`FinalizeInfo`) além dos campos existentes.
- [ ] `OutputInfo` e `FinalizeInfo` são records imutáveis no namespace `Domain.Models`.
- [ ] `dotnet build` passa sem erros após as alterações.

# Subtask-02: Atualizar mapeamento infra → domínio no ClientService

## Descrição
Atualizar o `VideoManagementClientService` para propagar os novos campos `DurationSec`, `FrameIntervalSec` e `ParallelChunks` ao construir o `VideoDetails` a partir da resposta da API, aplicando defaults seguros quando os valores forem ausentes ou inválidos.

## Arquivos a Modificar
- `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/VideoManagement/VideoManagementClientService.cs`

## Passos de Implementação
1. **Localizar o ponto de mapeamento** onde `VideoManagementVideoData` é convertido em `VideoDetails` (construção do record).
2. **Propagar os três novos campos:**
   - `DurationSec: data.DurationSec`
   - `FrameIntervalSec: data.FrameIntervalSec`
   - `ParallelChunks: data.ParallelChunks > 0 ? data.ParallelChunks : 1`
   - O guard de `ParallelChunks` neste ponto garante que o builder nunca receba um valor ≤ 0.
3. **Não alterar** nenhum outro trecho do service (tratamento de erro, headers, retry, etc.).

## Formas de Teste
1. **Build limpo:** `dotnet build` sem erros após a alteração.
2. **Teste de unidade (mock da API):** simular resposta com `parallelChunks=0` e confirmar que `VideoDetails.ParallelChunks` resulta em `1`.
3. **Teste de unidade (valores normais):** simular resposta com `durationSec=45`, `frameIntervalSec=5`, `parallelChunks=3` e confirmar mapeamento 1:1.

## Critérios de Aceite
- [ ] O mapeamento propaga `DurationSec`, `FrameIntervalSec` e `ParallelChunks` para `VideoDetails`.
- [ ] `parallelChunks=0` ou valor negativo na API resulta em `ParallelChunks=1` no domínio.
- [ ] Nenhuma outra lógica do ClientService é alterada.

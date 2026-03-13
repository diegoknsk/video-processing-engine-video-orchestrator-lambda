# Subtask-01: Enriquecer VideoManagementVideoData e VideoDetails com durationSec, frameIntervalSec e parallelChunks

## Descrição
Adicionar os campos `DurationSec`, `FrameIntervalSec` e `ParallelChunks` ao DTO de infra `VideoManagementVideoData` e ao record de domínio `VideoDetails`, garantindo que a informação retornada pela API Video Management chegue ao builder de payload.

## Arquivos a Modificar
- `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/VideoManagement/VideoManagementApiResponse.cs`
- `src/Core/VideoProcessing.VideoOrchestrator.Domain/Models/VideoDetails.cs`

## Passos de Implementação
1. **Adicionar campos no DTO de infra (`VideoManagementVideoData`):**
   - `[JsonPropertyName("durationSec")] public int DurationSec { get; init; }`
   - `[JsonPropertyName("frameIntervalSec")] public int FrameIntervalSec { get; init; }`
   - `[JsonPropertyName("parallelChunks")] public int ParallelChunks { get; init; }`
   - Manter default `= 0` para segundos e `= 1` para `ParallelChunks` (valor mínimo seguro).

2. **Adicionar campos no record de domínio (`VideoDetails`):**
   - Acrescentar `int DurationSec`, `int FrameIntervalSec`, `int ParallelChunks` como parâmetros do record.
   - Não alterar a ordem dos parâmetros existentes; adicionar ao final para minimizar impacto em código de chamada.

3. **Compilar a solution** e verificar que todos os locais onde `VideoDetails` é instanciado compilam — ajustar apenas os valores obrigatórios de construção, sem alterar lógica.

## Formas de Teste
1. **Build limpo:** `dotnet build` sem erros ou warnings de compilação.
2. **Desserialização manual:** montar JSON de exemplo (vide payload da story) e deserializar para `VideoManagementVideoData`; confirmar que `DurationSec=45`, `FrameIntervalSec=5` e `ParallelChunks=1` são populados corretamente.
3. **Teste de unidade (novo ou existente):** verificar que os campos novos do `VideoDetails` são acessíveis sem exceção quando construídos com valores padrão.

## Critérios de Aceite
- [ ] `VideoManagementVideoData` contém `DurationSec`, `FrameIntervalSec` e `ParallelChunks` com `[JsonPropertyName]` correto.
- [ ] `VideoDetails` expõe os três campos; builds sem erro após adição.
- [ ] JSON de exemplo deserializa corretamente: `DurationSec=45`, `FrameIntervalSec=5`, `ParallelChunks=1`.

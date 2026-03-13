# Subtask-03: Implementar divisão dinâmica de chunks no StepFunctionPayloadBuilder

## Descrição
Substituir o chunk fixo único no `StepFunctionPayloadBuilder` por um método privado estático `BuildChunks` que gera N chunks com faixas de tempo explícitas, calculadas com base em `DurationSec` e `ParallelChunks` do `VideoDetails`.

## Arquivo a Modificar
- `src/Core/VideoProcessing.VideoOrchestrator.Application/Builders/StepFunctionPayloadBuilder.cs`

## Passos de Implementação

1. **Criar método privado estático `BuildChunks`** dentro de `StepFunctionPayloadBuilder`:

```csharp
private static List<VideoChunk> BuildChunks(VideoDetails details, int intervalSec)
{
    var parallelChunks = details.ParallelChunks > 0 ? details.ParallelChunks : 1;
    var duration = details.DurationSec;

    if (duration <= 0)
        return [new(0, $"{details.VideoId}-chunk-0", 0, 0, intervalSec,
            $"videos/{details.UserId}/{details.VideoId}/manifests/chunk-0/",
            $"videos/{details.UserId}/{details.VideoId}/frames/chunk-0/")];

    var chunkDuration = (int)Math.Ceiling((double)duration / parallelChunks);
    var chunks = new List<VideoChunk>(parallelChunks);

    for (var i = 0; i < parallelChunks; i++)
    {
        var startSec = i * chunkDuration;
        var endSec = Math.Min((i + 1) * chunkDuration, duration);
        chunks.Add(new(
            ChunkIndex: i,
            ChunkId: $"{details.VideoId}-chunk-{i}",
            StartSec: startSec,
            EndSec: endSec,
            IntervalSec: intervalSec,
            ManifestPrefix: $"videos/{details.UserId}/{details.VideoId}/manifests/chunk-{i}/",
            FramesPrefix: $"videos/{details.UserId}/{details.VideoId}/frames/chunk-{i}/"
        ));
    }

    return chunks;
}
```

2. **Substituir a criação inline do chunk** no método `Build`:
   - Remover o bloco `var chunks = new List<VideoChunk> { new(...) }`.
   - Calcular `intervalSec`: `details.FrameIntervalSec > 0 ? details.FrameIntervalSec : outputOptions.FrameIntervalSec`.
   - Chamar `BuildChunks(details, intervalSec)`.

3. **Não alterar** nenhuma outra parte do método `Build` (output, zip, finalize, user).

## Algoritmo de divisão — exemplos esperados

| `durationSec` | `parallelChunks` | Chunks gerados |
|---------------|-----------------|----------------|
| 45            | 3               | [0,15), [15,30), [30,45] |
| 45            | 1               | [0,45] |
| 10            | 3               | [0,4), [4,8), [8,10] |
| 45            | 4               | [0,12), [12,24), [24,36), [36,45] |
| 0             | 3               | fallback: 1 chunk [0,0] |

> `chunkDuration = ceil(durationSec / parallelChunks)`. O último chunk sempre fecha em `durationSec`.

## Formas de Teste
1. **Build limpo:** `dotnet build` sem erros.
2. **Verificação unitária:** `durationSec=45`, `parallelChunks=3` → 3 chunks; primeiro `startSec=0`, `endSec=15`; último `endSec=45`.
3. **Verificação unitária:** `durationSec=45`, `parallelChunks=1` → 1 chunk `startSec=0`, `endSec=45` (sem `-1`).

## Critérios de Aceite
- [ ] `BuildChunks` é um método privado estático; nenhuma classe ou interface nova é criada.
- [ ] Com `parallelChunks=3` e `durationSec=45`, gera exatamente 3 chunks cobrindo `[0,15]`, `[15,30]`, `[30,45]`.
- [ ] `endSec` nunca é `-1` quando `durationSec` é conhecido e > 0.
- [ ] Restante do payload (`output`, `zip`, `finalize`, `user`) inalterado após a refatoração.

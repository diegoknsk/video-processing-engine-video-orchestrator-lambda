# Storie-06: Divisão Automática de Chunks no Payload do Step Function

## Status
- **Estado:** ⏸️ Pausada
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como orquestrador de processamento de vídeo, quero que o payload enviado ao Step Function contenha os chunks corretos calculados com base em `durationSec` e `parallelChunks`, para que o processamento paralelo real seja respeitado e cada lambda de extração receba apenas a faixa de tempo que lhe compete.

## Objetivo
Corrigir a geração de chunks no `StepFunctionPayloadBuilder`, substituindo o chunk fixo único (`startSec=0`, `endSec=-1`) por uma lista de N chunks com faixas de tempo explícitas e sem lacunas, calculadas a partir de `durationSec` e `parallelChunks` do vídeo.

## Contexto
- O orquestrador já busca os detalhes do vídeo via API interna (Video Management).
- O payload enviado ao Step Function já está bem estruturado com `contractVersion`, `output`, `finalize`, `user` e `chunks`.
- O modelo de domínio `VideoDetails` e o DTO `VideoManagementVideoData` **não possuem** ainda os campos `DurationSec`, `FrameIntervalSec` e `ParallelChunks`.
- O `StepFunctionPayloadBuilder` cria sempre 1 único chunk com `startSec=0` e `endSec=-1`, ignorando completamente a paralelização configurada no vídeo.

## Problema Atual
```csharp
// StepFunctionPayloadBuilder.cs — trecho incorreto
var chunks = new List<VideoChunk>
{
    new(
        ChunkIndex: 0,
        ChunkId: $"{details.VideoId}-chunk-0",
        StartSec: 0,
        EndSec: -1,           // ← nunca respeita a duração real
        IntervalSec: outputOptions.FrameIntervalSec,
        ManifestPrefix: $"videos/{details.UserId}/{details.VideoId}/manifests/chunk-0/",
        FramesPrefix: $"videos/{details.UserId}/{details.VideoId}/frames/chunk-0/"
    )
};
```

Impacto: independentemente de `parallelChunks` ser 1, 3 ou 5, o Step Function recebe sempre 1 chunk cobrindo o vídeo inteiro sem faixa de tempo definida.

## Proposta Técnica

### 1. Enriquecer o DTO de resposta da API (`VideoManagementVideoData`)
Adicionar os três campos ao DTO de infra que mapeia o JSON retornado pela API:

| Campo JSON       | Propriedade C#      | Tipo  |
|------------------|---------------------|-------|
| `durationSec`    | `DurationSec`       | `int` |
| `frameIntervalSec` | `FrameIntervalSec` | `int` |
| `parallelChunks` | `ParallelChunks`    | `int` |

### 2. Enriquecer o modelo de domínio (`VideoDetails`)
Adicionar os campos correspondentes ao record de domínio, mantendo compatibilidade com o mapeamento existente.

### 3. Atualizar o mapeamento infra → domínio
No `VideoManagementClientService` (ou equivalente), propagar os três novos campos ao construir `VideoDetails`.

### 4. Implementar a lógica de divisão no `StepFunctionPayloadBuilder`
Substituir o chunk fixo por um método privado estático `BuildChunks` que:

```
parallelChunks = max(1, details.ParallelChunks)
chunkDuration  = ceil(durationSec / parallelChunks)

para i de 0 até parallelChunks - 1:
    startSec = i * chunkDuration
    endSec   = min((i + 1) * chunkDuration, durationSec)
    — garantindo que o último chunk fecha exatamente em durationSec
```

**Regras de divisão:**
- `parallelChunks` inválido (≤ 0 ou nulo) → assumir `1`.
- `durationSec` ≤ 0 → gerar 1 chunk com `startSec=0` e `endSec=0` (vídeo sem duração conhecida; fallback seguro).
- Sem sobreposição: `endSec` de um chunk = `startSec` do próximo.
- Sem lacuna: o último chunk tem `endSec = durationSec`.
- `intervalSec` usa `details.FrameIntervalSec` se > 0; caso contrário, cai para `outputOptions.FrameIntervalSec` (fallback existente).
- `endSec` **nunca** será `-1` quando `durationSec` for conhecido.

### 5. Garantir cobertura com testes unitários
Cobrir os cenários principais no projeto de testes existente.

## Arquivos Afetados
| Arquivo | Operação |
|---------|----------|
| `src/Core/VideoProcessing.VideoOrchestrator.Domain/Models/VideoDetails.cs` | Adicionar campos `DurationSec`, `FrameIntervalSec`, `ParallelChunks` |
| `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/VideoManagement/VideoManagementApiResponse.cs` | Adicionar propriedades com `[JsonPropertyName]` |
| `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/VideoManagement/VideoManagementClientService.cs` | Propagar novos campos no mapeamento |
| `src/Core/VideoProcessing.VideoOrchestrator.Application/Builders/StepFunctionPayloadBuilder.cs` | Substituir chunk fixo por divisão dinâmica |
| Projeto de testes (xUnit) | Novos casos de teste para `StepFunctionPayloadBuilder` |

## Dependências e Riscos (para estimativa)
- **Dependências:** Story-04 (payload Step Function) e Story-05 (contrato Step Function) — ambas já concluídas.
- **Riscos:**
  - A API Video Management deve retornar `durationSec`, `frameIntervalSec` e `parallelChunks`; se omitidos, o mapeamento deve usar defaults seguros (0 / 1).
  - O processor downstream deve suportar `endSec` explícito (não `-1`). Verificar contrato do processor antes de implantar em produção.
  - Não refatorar outras partes do builder além da lógica de chunks.

## Subtasks
- [Subtask 01: Enriquecer VideoManagementVideoData e VideoDetails com durationSec, frameIntervalSec e parallelChunks](./subtask/Subtask-01-Enriquecer_VideoDetails_DTO.md)
- [Subtask 02: Atualizar mapeamento infra → domínio no ClientService](./subtask/Subtask-02-Atualizar_Mapeamento_ClientService.md)
- [Subtask 03: Implementar divisão dinâmica de chunks no StepFunctionPayloadBuilder](./subtask/Subtask-03-Implementar_Divisao_Chunks_Builder.md)
- [Subtask 04: Testes unitários para StepFunctionPayloadBuilder](./subtask/Subtask-04-Testes_Unitarios_Builder.md)

## Critérios de Aceite da História
- [ ] `VideoDetails` contém `DurationSec`, `FrameIntervalSec` e `ParallelChunks`; `VideoManagementVideoData` mapeia os campos correspondentes do JSON com `[JsonPropertyName]` correto.
- [ ] O mapeamento infra → domínio propaga os três novos campos; campos ausentes na resposta da API resultam em defaults seguros (0 para segundos, 1 para chunks).
- [ ] Com `parallelChunks=3` e `durationSec=45`, o builder gera exatamente 3 chunks cobrindo `[0,15)`, `[15,30)` e `[30,45]` sem lacunas nem sobreposição.
- [ ] Com `parallelChunks=1`, o builder gera 1 chunk com `startSec=0` e `endSec=durationSec` (sem `-1`).
- [ ] `intervalSec` de cada chunk usa `details.FrameIntervalSec` quando > 0; usa `outputOptions.FrameIntervalSec` como fallback quando `FrameIntervalSec` ≤ 0.
- [ ] `parallelChunks` inválido (≤ 0) é tratado como 1; `durationSec` ≤ 0 gera 1 chunk com `startSec=0` e `endSec=0`.
- [ ] O restante do payload (`contractVersion`, `output`, `zip`, `finalize`, `user`) permanece inalterado — sem regressão.
- [ ] Testes unitários passando; cobertura ≥ 80% para `StepFunctionPayloadBuilder`.

## Observações Técnicas
- Usar aritmética inteira (`(int)Math.Ceiling`) para `chunkDuration`; não introduzir `double` desnecessário.
- O `endSec` do último chunk deve ser `durationSec`, não `(parallelChunks * chunkDuration)` — evitar ultrapassar a duração real por arredondamento.
- Manter `StepFunctionPayloadBuilder` como classe estática; extrair apenas um método privado `BuildChunks`.
- Não alterar o contrato de `VideoChunk` (record já adequado).
- Não introduzir novos pacotes/dependências; solução usa apenas BCL.

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

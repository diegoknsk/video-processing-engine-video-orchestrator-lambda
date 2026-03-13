# Storie-05: Adaptar Payload do Orquestrador ao Novo Contrato da Step Function

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como Lambda Orquestrador, quero gerar um payload compatível com o novo contrato esperado pela Step Function — incluindo campos por chunk (`chunkId`, `startSec`, `endSec`, `intervalSec`, `manifestPrefix`, `framesPrefix`), bloco `output` com buckets configuráveis e bloco `finalize` com os dados necessários para a etapa de finalização — para que o pipeline de processamento possa ser executado sem falhas de contrato e o lambda de finalize receba os dados corretos sem necessidade de busca adicional.

## Objetivo
Adaptar o payload enviado pelo orquestrador à Step Function, enriquecendo os chunks com informações de tempo e de paths de manifestos e frames, adicionando o bloco `output` com buckets configuráveis (com defaults controlados), adicionando o bloco `finalize` com todos os dados necessários para a etapa de finalização, e extraindo os valores de bucket de variáveis de ambiente/configuração — sem hardcode rígido e sem quebrar a arquitetura existente.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, AWS Lambda, AWS Step Functions
- **Arquivos afetados/criados:**
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/Settings/OutputOptions.cs` *(novo)*
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/DependencyInjection.cs` *(atualização — registrar OutputOptions)*
  - `src/Core/VideoProcessing.VideoOrchestrator.Domain/Models/StepFunctionPayload.cs` *(atualização — novos campos em VideoChunk, novos records OutputInfo e FinalizeInfo)*
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/Builders/StepFunctionPayloadBuilder.cs` *(atualização — lógica de montagem com os novos campos)*
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/UseCases/OrchestrateVideoProcessingUseCase.cs` *(atualização — passa OutputOptions ao builder)*
  - `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/` *(novos e atualizados testes)*
- **Componentes criados/modificados:**
  - `OutputOptions` — nova classe de configuração com `FramesBucket`, `ManifestBucket`, `ZipBucket`, `FrameIntervalSec` e `OrdenaAutomaticamente`, todos com defaults controlados (seção `OUTPUT`, env `OUTPUT__*`)
  - `VideoChunk` — atualizado com: `ChunkId`, `StartSec`, `EndSec`, `IntervalSec`, `ManifestPrefix`, `FramesPrefix` (mantém `ChunkIndex` e remove `OutputPath` — substituído por `FramesPrefix`)
  - `OutputInfo` — novo record com `FramesBucket` e `ManifestBucket`
  - `FinalizeInfo` — novo record com `FramesBucket`, `FramesBasePrefix`, `OutputBucket`, `VideoId`, `OutputBasePrefix` e `OrdenaAutomaticamente`
  - `StepFunctionPayload` — atualizado: adiciona `ContractVersion`, `Output` (`OutputInfo`) e `Finalize` (`FinalizeInfo`)
  - `StepFunctionPayloadBuilder` — refatorado para receber `OutputOptions` e montar todos os novos campos
  - `OrchestrateVideoProcessingUseCase` — recebe `IOptions<OutputOptions>` via DI e repassa ao builder
- **Pacotes/Dependências:** nenhum novo pacote externo necessário

## Novo Contrato do Payload (target)

```json
{
  "contractVersion": "1.0",
  "executionId": "exec-{videoId}-{timestamp}",
  "video": {
    "videoId": "...",
    "userId": "...",
    "title": "...",
    "s3Bucket": "...",
    "s3Key": "...",
    "outputPrefix": "videos/{userId}/{videoId}/frames/",
    "chunks": [
      {
        "chunkIndex": 0,
        "chunkId": "{videoId}-chunk-0",
        "startSec": 0,
        "endSec": -1,
        "intervalSec": 1,
        "manifestPrefix": "videos/{userId}/{videoId}/manifests/chunk-0/",
        "framesPrefix": "videos/{userId}/{videoId}/frames/chunk-0/"
      }
    ]
  },
  "output": {
    "framesBucket": "<OUTPUT__FRAMES_BUCKET ou default>",
    "manifestBucket": "<OUTPUT__MANIFEST_BUCKET ou default>"
  },
  "zip": {
    "outputBucket": "<OUTPUT__ZIP_BUCKET ou default>",
    "outputKey": "videos/{userId}/{videoId}/output.zip"
  },
  "finalize": {
    "framesBucket": "<OUTPUT__FRAMES_BUCKET>",
    "framesBasePrefix": "videos/{userId}/{videoId}/frames/",
    "outputBucket": "<OUTPUT__ZIP_BUCKET>",
    "videoId": "...",
    "outputBasePrefix": "{userId}/{videoId}",
    "ordenaAutomaticamente": true
  },
  "user": {
    "name": "...",
    "email": "..."
  }
}
```

## Regras de Configuração e Defaults

| Variável de ambiente         | Propriedade em OutputOptions | Default controlado                              |
|-----------------------------|------------------------------|-------------------------------------------------|
| `OUTPUT__FRAMES_BUCKET`     | `FramesBucket`               | `"video-processing-engine-dev-images"`          |
| `OUTPUT__MANIFEST_BUCKET`   | `ManifestBucket`             | `"video-processing-engine-dev-images"`          |
| `OUTPUT__ZIP_BUCKET`        | `ZipBucket`                  | `"video-processing-engine-dev-zip"`             |
| `OUTPUT__FRAME_INTERVAL_SEC`| `FrameIntervalSec`           | `1`                                             |
| `OUTPUT__ORDENA_AUTOMATICAMENTE` | `OrdenaAutomaticamente` | `true`                                          |

> Nenhuma variável é `[Required]`; todas têm `init` com valor default, garantindo que a Lambda funcione mesmo sem variáveis de ambiente configuradas no ambiente de dev.

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-04 concluída — o modelo `StepFunctionPayload` e `StepFunctionPayloadBuilder` criados nela são a base da refatoração desta story.
  - O contrato do lambda processor e do Step Function Map State deve ser validado contra o novo payload antes do merge.
- **Riscos/Pré-condições:**
  - `endSec: -1` como convenção de "até o fim do vídeo" precisa ser validada contra o comportamento esperado pelo lambda processor; se o processor não suportar esse valor, a strategy de chunking deve ser ajustada.
  - A remoção de `OutputPath` de `VideoChunk` é breaking change em testes e quaisquer consumidores do modelo — todos devem ser atualizados nesta story.
  - O bloco `finalize` replica dados presentes em outros blocos (redundância intencional para desacoplar a etapa de finalização); isso deve ser documentado no código.

## Subtasks
- [Subtask 01: Criar OutputOptions com defaults e registrar no DI](./subtask/Subtask-01-OutputOptions_Configuracao.md)
- [Subtask 02: Atualizar modelos de domínio — VideoChunk, OutputInfo, FinalizeInfo, StepFunctionPayload](./subtask/Subtask-02-Modelos_Dominio.md)
- [Subtask 03: Refatorar StepFunctionPayloadBuilder com novo contrato](./subtask/Subtask-03-PayloadBuilder_Refactor.md)
- [Subtask 04: Atualizar UseCase para injetar OutputOptions e propagar ao builder](./subtask/Subtask-04-UseCase_OutputOptions.md)
- [Subtask 05: Testes unitários — atualizar e criar cobertura do novo contrato](./subtask/Subtask-05-Testes_Unitarios.md)

## Critérios de Aceite da História
- [ ] O payload serializado pela Step Function contém obrigatoriamente: `contractVersion`, `executionId`, `video` (com `chunks`), `output`, `zip`, `finalize` e `user` — ausência de qualquer bloco ou campo obrigatório é falha de teste.
- [ ] Cada item de `video.chunks` contém: `chunkIndex`, `chunkId`, `startSec`, `endSec`, `intervalSec`, `manifestPrefix` e `framesPrefix` — o campo `outputPath` não existe mais no modelo serializado.
- [ ] O bloco `output` contém `framesBucket` e `manifestBucket` vindos de `OutputOptions`; o bloco `zip` usa `outputBucket` de `OutputOptions.ZipBucket` — nenhum nome de bucket está hardcoded em código de produção.
- [ ] Quando nenhuma variável de ambiente `OUTPUT__*` estiver definida, o builder utiliza os defaults controlados definidos em `OutputOptions` — validado por teste unitário sem variáveis de ambiente.
- [ ] O bloco `finalize` contém: `framesBucket`, `framesBasePrefix` (caminho base dos frames), `outputBucket`, `videoId`, `outputBasePrefix` e `ordenaAutomaticamente` — compatível com o contrato esperado pelo lambda de finalize.
- [ ] `dotnet build` e `dotnet test` passam sem warnings ou erros após as alterações.
- [ ] Testes unitários cobrem: montagem com defaults (sem env vars), montagem com todas as variáveis configuradas, verificação de todos os campos obrigatórios nos blocos `output`, `finalize` e em cada `chunk`; cobertura ≥ 80% nos componentes alterados.
- [ ] Nenhum valor de bucket está hardcoded em `StepFunctionPayloadBuilder` ou `OrchestrateVideoProcessingUseCase` — todos os buckets vêm de `OutputOptions`.

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

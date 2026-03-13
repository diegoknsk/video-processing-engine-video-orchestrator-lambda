# Subtask-04: Testes unitários para StepFunctionPayloadBuilder

## Descrição
Criar (ou complementar) testes unitários para `StepFunctionPayloadBuilder`, cobrindo os cenários de divisão de chunks com xUnit e garantindo cobertura ≥ 80% na classe.

## Arquivo(s) a Criar/Modificar
- Projeto de testes existente (xUnit) — classe `StepFunctionPayloadBuilderTests`.

## Passos de Implementação
1. **Localizar ou criar** a classe de testes `StepFunctionPayloadBuilderTests` no projeto de testes xUnit.
2. **Implementar os cenários abaixo** usando `[Theory] + [InlineData]` para os casos paramétricos:

   | Cenário | `durationSec` | `parallelChunks` | Expectativa |
   |---------|--------------|-----------------|-------------|
   | Divisão exata | 45 | 3 | 3 chunks; `[0,15]`, `[15,30]`, `[30,45]` |
   | Chunk único | 45 | 1 | 1 chunk; `startSec=0`, `endSec=45` |
   | Divisão com resto | 10 | 3 | 3 chunks; `[0,4]`, `[4,8]`, `[8,10]` |
   | parallelChunks inválido (0) | 30 | 0 | 1 chunk; `startSec=0`, `endSec=30` |
   | durationSec zero | 0 | 3 | 1 chunk fallback; `startSec=0`, `endSec=0` |
   | intervalSec do vídeo | 45 | 1 | `IntervalSec` = `FrameIntervalSec` do details |
   | intervalSec fallback | 45 | 1 | `details.FrameIntervalSec=0` → usa `outputOptions.FrameIntervalSec` |

3. **Verificar** que o restante do payload (`ContractVersion`, `Output`, `Zip`, `Finalize`, `User`) permanece correto em pelo menos 1 teste de smoke end-to-end do `Build`.

## Formas de Teste
1. **`dotnet test`** sem falhas — todos os casos acima passando.
2. **Revisão manual** do relatório de cobertura: `StepFunctionPayloadBuilder` ≥ 80%.
3. **Edge case:** chunk único com `parallelChunks=1` confirma ausência de `endSec=-1`.

## Critérios de Aceite
- [ ] Todos os 7 cenários da tabela têm ao menos 1 assertion verificando quantidade de chunks e valores de `startSec`/`endSec`.
- [ ] `dotnet test` retorna 0 falhas.
- [ ] Cobertura de `StepFunctionPayloadBuilder` ≥ 80% (medida pelo projeto de testes existente).
- [ ] Nenhum teste de cenário anterior (se existir) é quebrado.

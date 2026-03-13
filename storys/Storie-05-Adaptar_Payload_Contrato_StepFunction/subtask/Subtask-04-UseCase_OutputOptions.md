# Subtask-04: Atualizar UseCase para injetar OutputOptions e propagar ao builder

## Descrição
Atualizar `OrchestrateVideoProcessingUseCase` para receber `IOptions<OutputOptions>` via construtor primário (DI) e repassar o valor ao `StepFunctionPayloadBuilder.Build`, garantindo que os buckets de saída e parâmetros de configuração fluam corretamente do container até o payload final.

## Passos de Implementação

1. **Adicionar dependência `IOptions<OutputOptions>`** no construtor primário de `OrchestrateVideoProcessingUseCase`:
   ```csharp
   public sealed class OrchestrateVideoProcessingUseCase(
       IStepFunctionService stepFunctionService,
       IOptions<OutputOptions> outputOptions,
       ILogger<OrchestrateVideoProcessingUseCase> logger) : IOrchestrateVideoProcessingUseCase
   ```

2. **Atualizar a chamada ao builder** dentro do método `ExecuteAsync`:
   ```csharp
   var payload = StepFunctionPayloadBuilder.Build(videoDetails, executionId, outputOptions.Value);
   ```

3. **Verificar que `DependencyInjection.cs`** (CrossCutting) já registra `IOptions<OutputOptions>` após a Subtask-01 — nenhuma alteração adicional no DI deve ser necessária aqui, mas confirmar que `OrchestrateVideoProcessingUseCase` está registrado como transient/scoped no container.

4. **Atualizar o namespace/using** se necessário para que `OutputOptions` seja visível no projeto `Application` — verificar se é necessário adicionar referência de projeto entre `Application` e `Infra.CrossCutting` ou se `OutputOptions` deve ser reposicionado em um projeto mais central (ex.: `Domain`).
   > **Decisão de arquitetura:** se `Application` não pode depender de `Infra.CrossCutting`, criar um record simples `OutputSettings` no `Application` e mapear `OutputOptions` → `OutputSettings` no bootstrap/CrossCutting. Caso o projeto já tenha essa dependência (verificar o `.csproj`), usar `OutputOptions` diretamente.

## Formas de Teste

1. **Teste unitário do UseCase:** mockar `IStepFunctionService`, `IOptions<OutputOptions>` e `ILogger`; verificar que `ExecuteAsync` chama `StartExecutionAsync` com um payload contendo os valores de bucket provenientes das options mockadas.
2. **Teste de integração leve (bootstrap):** resolver `IOrchestrateVideoProcessingUseCase` do container e verificar que nenhuma exceção de DI é lançada.
3. **Verificação de compilação:** confirmar que `Application` compila com a nova dependência de `OutputOptions` sem violar as regras de camada (Application → Domain apenas; Application não depende de Infra).

## Critérios de Aceite

- [ ] `OrchestrateVideoProcessingUseCase` recebe `IOptions<OutputOptions>` sem violar as regras de camada Clean Architecture do projeto.
- [ ] O payload gerado em `ExecuteAsync` contém os buckets corretos vindos das options injetadas.
- [ ] `dotnet build` e `dotnet test` passam sem erros após a alteração.
- [ ] Não há nenhum valor de bucket hardcoded no UseCase.

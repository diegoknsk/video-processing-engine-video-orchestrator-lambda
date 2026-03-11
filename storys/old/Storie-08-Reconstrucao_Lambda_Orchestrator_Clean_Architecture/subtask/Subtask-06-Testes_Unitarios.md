# Subtask-06: Criar projeto de testes unitários e cobertura mínima

## Descrição
Implementar os testes unitários no projeto `VideoProcessing.VideoOrchestrator.UnitTests`, cobrindo os dois componentes mais críticos: `OrchestratorEventAdapter` (parsing de SQS vs. JSON direto) e `OrchestratorHandler` (validação e delegação ao use case mockado). Atingir cobertura mínima de 80%.

## Passos de Implementação

1. **Confirmar pacotes do projeto de testes:**
   ```xml
   <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.x" />
   <PackageReference Include="xunit" Version="2.9.2" />
   <PackageReference Include="xunit.runner.visualstudio" Version="2.9.2" />
   <PackageReference Include="Moq" Version="4.20.72" />
   <PackageReference Include="FluentAssertions" Version="6.12.0" />
   <PackageReference Include="coverlet.collector" Version="6.0.4" />
   ```

2. **Testes do `OrchestratorEventAdapter`:**
   Criar `tests/VideoProcessing.VideoOrchestrator.UnitTests/Lambda/OrchestratorEventAdapterTests.cs`:
   - `FromRawEvent_WithSqsEnvelope_ShouldReturnOneEvent` — payload com `Records[].body` válido → lista com 1 evento
   - `FromRawEvent_WithDirectPayload_ShouldReturnOneEvent` — JSON direto com `jobId` e `correlationId` → lista com 1 evento
   - `FromRawEvent_WithEmptySqsRecords_ShouldReturnEmptyList` — `Records: []` → lista vazia
   - `FromRawEvent_WithNullBody_ShouldReturnEmptyList` — record SQS com body nulo → lista vazia
   - `FromRawEvent_WithInvalidJson_ShouldReturnEmptyList` — JSON corrompido no body → lista vazia
   - `FromRawEvent_WithMultipleSqsRecords_ShouldReturnAllEvents` — 2 records → lista com 2 eventos

3. **Testes do `OrchestratorHandler`:**
   Criar `tests/VideoProcessing.VideoOrchestrator.UnitTests/Lambda/OrchestratorHandlerTests.cs`:
   - `HandleAsync_WithValidEvent_ShouldReturnStatusCode200` — use case mock retorna `OrchestratorJobResponseModel` → resposta 200
   - `HandleAsync_WhenUseCaseThrows_ShouldReturnValidationError` — use case mock lança `Exception` → resposta >= 400
   - `HandleAsync_ShouldMapEventToInputModelCorrectly` — verificar que `JobId` e `CorrelationId` são mapeados corretamente para o `OrchestratorJobInputModel` passado ao use case

4. **Testes do `OrchestratorJobUseCase` (Application):**
   Criar `tests/VideoProcessing.VideoOrchestrator.UnitTests/Application/OrchestratorJobUseCaseTests.cs`:
   - `ExecuteAsync_WithValidInput_ShouldReturnMockedResponse` — verificar que retorna `OrchestratorJobResponseModel` com `Status = "Processed"` sem exceção

5. **Configurar `coverlet` no `.csproj` de testes:**
   ```xml
   <PropertyGroup>
     <CollectCoverage>true</CollectCoverage>
     <CoverletOutputFormat>cobertura</CoverletOutputFormat>
   </PropertyGroup>
   ```

6. **Executar testes e verificar cobertura:**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
   Verificar que cobertura nos arquivos chave (`OrchestratorEventAdapter.cs`, `OrchestratorHandler.cs`, `OrchestratorJobUseCase.cs`) seja ≥ 80%.

## Formas de Teste

1. `dotnet test` deve executar todos os testes com 0 falhas
2. Relatório de cobertura (`coverage.cobertura.xml`) deve indicar ≥ 80% de cobertura nas classes principais
3. Cada cenário de `OrchestratorEventAdapterTests` testado individualmente via `dotnet test --filter DisplayName~...`

## Critérios de Aceite

- [ ] Mínimo 10 testes unitários criados (6 do adapter + 3 do handler + 1 do use case)
- [ ] `dotnet test` retorna 0 falhas e 0 erros
- [ ] Cobertura de código ≥ 80% nas classes `OrchestratorEventAdapter`, `OrchestratorHandler` e `OrchestratorJobUseCase`
- [ ] Todos os cenários de parsing SQS vs. JSON direto estão cobertos por testes
- [ ] Mock de `IOrchestratorJobUseCase` via `Moq` utilizado nos testes do handler (sem chamar implementação real)

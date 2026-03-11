# Subtask-02: Implementar Domain e Application layers (contratos e use case mockado)

## Descrição
Criar os contratos e modelos mínimos nas camadas `Domain` e `Application`, seguindo Clean Architecture. Como o resultado final desta story é mockado, o use case apenas retorna uma resposta de echo sem regra de negócio real. Isso prepara as interfaces (ports) que as camadas de infra e a borda lambda irão implementar/consumir.

## Passos de Implementação

1. **Domain — Entidade mínima e enum:**
   Criar `src/Core/VideoProcessing.VideoOrchestrator.Domain/`:
   - `Entities/OrchestratorJob.cs` — entidade mínima com `JobId (Guid)`, `CorrelationId (string)`, `Status (JobStatus)`, `CreatedAt (DateTimeOffset)`. Sem ORM annotations, sem dependências externas.
   - `Enums/JobStatus.cs` — enum `Pending`, `Processing`, `Completed`, `Failed`

2. **Application — InputModel e ResponseModel:**
   Criar `src/Core/VideoProcessing.VideoOrchestrator.Application/Models/`:
   - `InputModels/OrchestratorJobInputModel.cs` — record com `JobId (Guid)`, `CorrelationId (string)`, `Payload (string?)` (dados brutos da mensagem SQS)
   - `ResponseModels/OrchestratorJobResponseModel.cs` — record com `JobId (Guid)`, `CorrelationId (string)`, `Status (string)`, `Message (string)`

3. **Application — Port (interface do use case):**
   Criar `src/Core/VideoProcessing.VideoOrchestrator.Application/Ports/`:
   - `IOrchestratorJobUseCase.cs` — interface com `Task<OrchestratorJobResponseModel> ExecuteAsync(OrchestratorJobInputModel input, CancellationToken cancellationToken = default)`

4. **Application — Use case mockado:**
   Criar `src/Core/VideoProcessing.VideoOrchestrator.Application/UseCases/ProcessJob/`:
   - `OrchestratorJobUseCase.cs` — implementação que loga e retorna imediatamente `OrchestratorJobResponseModel` com `Status = "Processed"`, `Message = "Mock: job recebido e processado (stub)"`. Sem chamada a repositório ou serviço externo.

5. **Application — DependencyInjection:**
   Criar `src/Core/VideoProcessing.VideoOrchestrator.Application/DependencyInjection/ApplicationServiceExtensions.cs` com método de extensão `AddApplicationServices(this IServiceCollection services)` que registra o use case como `Singleton`.

6. **Adicionar pacotes ao projeto Application:**
   - `Microsoft.Extensions.DependencyInjection.Abstractions` (via metapacote da versão 10.0.x)
   - `Microsoft.Extensions.Logging.Abstractions` (para `ILogger<T>` no use case)

## Formas de Teste

1. `dotnet build` no projeto `Application` deve compilar sem erros
2. Verificar que `Domain` não referencia nenhum pacote externo (zero `<PackageReference>` no `.csproj`)
3. Inspecionar o use case mockado: o método `ExecuteAsync` deve retornar `OrchestratorJobResponseModel` sem chamar nenhuma dependência externa

## Critérios de Aceite

- [ ] `OrchestratorJob` (entidade) e `JobStatus` (enum) criados no Domain sem dependências externas
- [ ] `OrchestratorJobInputModel` e `OrchestratorJobResponseModel` criados como `record` no Application
- [ ] Interface `IOrchestratorJobUseCase` definida no Application com assinatura async
- [ ] `OrchestratorJobUseCase` implementa a interface e retorna resposta mockada sem erros
- [ ] `Domain.csproj` não contém nenhum `<PackageReference>`
- [ ] `dotnet build` em ambos os projetos retorna 0 erros

# Subtask-07: Validar build local, testes passando e execução no Lambda Test Tool

## Descrição
Validação final da story: garantir que o `dotnet build` e `dotnet test` passam na solução completa, que o Lambda Test Tool abre corretamente com F5 no Visual Studio, e que ambos os payloads de exemplo (envelope SQS e JSON direto) retornam `statusCode: 200` com os dados mockados esperados.

## Passos de Implementação

1. **Build completo da solução:**
   ```bash
   dotnet build src/VideoOrchestrator.sln -c Debug
   ```
   Verificar: `Build succeeded`, 0 erros, 0 warnings críticos.

2. **Executar todos os testes:**
   ```bash
   dotnet test src/VideoOrchestrator.sln --collect:"XPlat Code Coverage" --results-directory ./coverage
   ```
   Verificar: todos os testes passam, cobertura ≥ 80%.

3. **Build de publish para validar que o zip de deploy funcionaria:**
   ```bash
   dotnet publish src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj -c Release -r linux-x64 --self-contained false -o ./publish-test
   ```
   Verificar que `VideoProcessing.VideoOrchestrator.Lambda.dll` e `.runtimeconfig.json` estão presentes no output.

4. **Teste manual com Lambda Test Tool:**
   - Abrir Visual Studio, selecionar projeto `VideoProcessing.VideoOrchestrator.Lambda`
   - Selecionar perfil `Lambda Test Tool` e pressionar F5
   - No browser da test tool, carregar `sqs-envelope.json` das saved requests e executar
   - Verificar resposta: `{"statusCode":200,"jobId":"550e8400-...","correlationId":"corr-test-001","status":"Processed","message":"Mock: job recebido e processado (stub)"}`
   - Repetir com `direct-payload.json` — mesma estrutura de resposta

5. **Validar logs no console do Lambda Test Tool:**
   - Deve aparecer log estruturado com nível `Information` indicando o processamento do evento
   - Nenhuma exceção não tratada deve aparecer nos logs

6. **Revisão final da estrutura de pastas:**
   Confirmar que a estrutura final é:
   ```
   src/
   ├── Core/
   │   ├── VideoProcessing.VideoOrchestrator.Domain/
   │   └── VideoProcessing.VideoOrchestrator.Application/
   ├── Infra/
   │   ├── VideoProcessing.VideoOrchestrator.Infra.CrossCutting/
   │   └── VideoProcessing.VideoOrchestrator.Infra.Data/
   └── InterfacesExternas/
       └── VideoProcessing.VideoOrchestrator.Lambda/
           ├── Function.cs
           ├── Startup.cs
           ├── IOrchestratorEventAdapter.cs / OrchestratorEventAdapter.cs
           ├── IOrchestratorHandler.cs / OrchestratorHandler.cs
           ├── Models/
           │   ├── OrchestratorLambdaEvent.cs
           │   └── OrchestratorLambdaResponse.cs
           ├── aws-lambda-tools-defaults.json
           ├── Properties/launchSettings.json
           └── .lambda-test-tool/SavedRequests/
               ├── sqs-envelope.json
               └── direct-payload.json
   tests/
   └── VideoProcessing.VideoOrchestrator.UnitTests/
   ```

## Formas de Teste

1. `dotnet build src/VideoOrchestrator.sln` — deve retornar `Build succeeded`
2. `dotnet test src/VideoOrchestrator.sln` — deve retornar 0 falhas
3. Lambda Test Tool abre no browser e executa `sqs-envelope.json` retornando `statusCode: 200`

## Critérios de Aceite

- [ ] `dotnet build` na solução completa retorna `Build succeeded` com 0 erros
- [ ] `dotnet test` retorna 0 falhas com cobertura ≥ 80%
- [ ] `dotnet publish` gera artefato válido com `.dll` e `.runtimeconfig.json`
- [ ] Lambda Test Tool executa com payload SQS envelope e retorna `statusCode: 200`
- [ ] Lambda Test Tool executa com payload JSON direto e retorna `statusCode: 200`
- [ ] Logs do Lambda Test Tool mostram logging estruturado sem exceções não tratadas
- [ ] Estrutura de pastas e arquivos conforme especificado nessa subtask

# Subtask-03: Validar pipeline — dry-run local e execução via workflow_dispatch

## Descrição
Validar o workflow atualizado end-to-end: primeiro simulando localmente o build/publish com o novo caminho de projeto, depois acionando o pipeline real via `workflow_dispatch` no GitHub Actions e confirmando que a função Lambda no AWS foi atualizada corretamente e responde com `statusCode: 200`.

## Passos de Implementação

1. **Dry-run local — simular os steps do pipeline:**
   Executar manualmente na raiz do repositório os mesmos comandos do workflow:
   ```bash
   # Restore
   dotnet restore src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj

   # Build Release
   dotnet build src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj -c Release --no-restore

   # Publish (mesmo comando do workflow)
   dotnet publish src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj -c Release -r linux-x64 --self-contained false -o ./publish-dry-run

   # Verificar artefatos
   ls ./publish-dry-run/
   ```
   Confirmar que `VideoProcessing.VideoOrchestrator.Lambda.dll` e `VideoProcessing.VideoOrchestrator.Lambda.runtimeconfig.json` estão presentes.

2. **Validar tamanho do zip (estimativa):**
   ```bash
   cd ./publish-dry-run && zip -r ../lambda-dry-run.zip . && cd ..
   ls -lh lambda-dry-run.zip
   ```
   O zip deve ter tamanho razoável (tipicamente < 50 MB para Lambda .NET sem self-contained).

3. **Acionar o pipeline via `workflow_dispatch`:**
   - No GitHub, acessar `Actions → Deploy Lambda → Run workflow`
   - Selecionar branch `dev` (ou `main`)
   - Clicar em `Run workflow` e monitorar a execução

4. **Monitorar cada step no GitHub Actions:**
   Verificar na UI do Actions que todos os steps completam com ✅:
   - Checkout
   - Setup .NET
   - Restore
   - Build
   - Publish
   - Create deployment package
   - Configure AWS credentials
   - Deploy to Lambda
   - Wait for Lambda update to complete
   - Update Lambda handler

5. **Verificar deploy no AWS Console:**
   - Acessar `AWS Console → Lambda → [function-name]`
   - Verificar `Configuration → General` → `Handler` = `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`
   - Verificar `Code` → `Last modified` mostra timestamp recente

6. **Invocar a Lambda no AWS para smoke test:**
   Usar o painel `Test` do AWS Console com o payload:
   ```json
   {
     "Records": [
       {
         "messageId": "smoke-test-001",
         "receiptHandle": "smoke-receipt",
         "body": "{\"jobId\":\"550e8400-e29b-41d4-a716-446655440000\",\"correlationId\":\"smoke-test-001\",\"payload\":\"smoke-test\"}",
         "attributes": {},
         "messageAttributes": {},
         "md5OfBody": "",
         "eventSource": "aws:sqs",
         "eventSourceARN": "arn:aws:sqs:us-east-1:123456789012:video-orchestrator-queue",
         "awsRegion": "us-east-1"
       }
     ]
   }
   ```
   Verificar que a resposta contém `"statusCode": 200`.

## Formas de Teste

1. Dry-run local retorna `Publish succeeded` com os artefatos corretos
2. Workflow no GitHub Actions executa com todos os steps em verde (✅)
3. Invocação da Lambda via AWS Console Test retorna `statusCode: 200`

## Critérios de Aceite

- [ ] Dry-run local de `dotnet publish` gera `.dll` e `.runtimeconfig.json` no output
- [ ] Workflow `Deploy Lambda` executa com sucesso (todos os steps ✅) via `workflow_dispatch`
- [ ] Handler da função Lambda no AWS confirma `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler` após deploy
- [ ] Smoke test no AWS Console retorna `statusCode: 200` com payload SQS
- [ ] Nenhum erro nas etapas `Wait for Lambda update to complete` ou `Update Lambda handler`
- [ ] CloudWatch Logs (opcional) mostram logs estruturados do processamento do evento

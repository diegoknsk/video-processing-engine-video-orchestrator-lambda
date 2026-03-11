# Subtask-05: Configurar aws-lambda-tools-defaults.json e launchSettings.json para teste local

## Descrição
Configurar os arquivos `aws-lambda-tools-defaults.json` e `Properties/launchSettings.json` no projeto Lambda para permitir execução e teste local via `dotnet-lambda-test-tool-10.0`, e preparar uma request de exemplo SQS salva na pasta `.lambda-test-tool/SavedRequests/` para facilitar o teste manual.

## Passos de Implementação

1. **Criar `aws-lambda-tools-defaults.json`** em `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/`:
   ```json
   {
     "Information": [
       "Configuração para o Lambda Test Tool e deploy.",
       "Working directory deve ser a pasta deste projeto (Lambda).",
       "Handler: VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler"
     ],
     "profile": "default",
     "region": "us-east-1",
     "configuration": "Debug",
     "function-handler": "VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler"
   }
   ```

2. **Criar `Properties/launchSettings.json`** em `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/Properties/`:
   ```json
   {
     "profiles": {
       "Lambda Test Tool": {
         "commandName": "Executable",
         "executablePath": "C:\\Users\\diego\\.dotnet\\tools\\dotnet-lambda-test-tool-10.0.exe",
         "workingDirectory": "$(ProjectDir)",
         "environmentVariables": {
           "AWS_REGION": "us-east-1",
           "Lambda__AwsRegion": "us-east-1"
         }
       }
     }
   }
   ```
   > Nota: o `executablePath` aponta para a instalação local do `dotnet-lambda-test-tool-10.0`. Ajustar se o caminho for diferente na máquina.

3. **Criar pasta `.lambda-test-tool/SavedRequests/`** no projeto Lambda e adicionar dois arquivos de request de exemplo:
   - `sqs-envelope.json` — payload no formato envelope SQS (com `Records[].body`) contendo um `OrchestratorLambdaEvent` de exemplo:
     ```json
     {
       "Records": [
         {
           "messageId": "test-message-id-001",
           "receiptHandle": "test-receipt-handle",
           "body": "{\"jobId\":\"550e8400-e29b-41d4-a716-446655440000\",\"correlationId\":\"corr-test-001\",\"payload\":\"video-123.mp4\"}",
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
   - `direct-payload.json` — payload JSON direto (sem envelope SQS):
     ```json
     {
       "jobId": "550e8400-e29b-41d4-a716-446655440000",
       "correlationId": "corr-test-002",
       "payload": "video-456.mp4"
     }
     ```

4. **Verificar instalação do `dotnet-lambda-test-tool-10.0`:**
   Confirmar que a ferramenta está instalada globalmente:
   ```bash
   dotnet tool list -g | grep lambda-test-tool
   ```
   Se não estiver instalada:
   ```bash
   dotnet tool install -g Amazon.Lambda.TestTool
   ```

5. **Testar que o perfil de launch funciona:**
   - Abrir o projeto `VideoProcessing.VideoOrchestrator.Lambda` no Visual Studio ou executar `dotnet-lambda-test-tool-10.0` diretamente
   - Selecionar o request `sqs-envelope.json` nas saved requests
   - Executar e verificar que a resposta retorna `{"statusCode":200,...}`

## Formas de Teste

1. Abrir o projeto no Visual Studio, selecionar perfil `Lambda Test Tool` e iniciar com F5 — a UI do test tool deve abrir no browser
2. Enviar o payload `sqs-envelope.json` e verificar que a resposta tem `statusCode: 200` com echo dos campos `jobId` e `correlationId`
3. Enviar o payload `direct-payload.json` e verificar o mesmo resultado

## Critérios de Aceite

- [ ] `aws-lambda-tools-defaults.json` criado com `function-handler` correto no formato `Assembly::Namespace.Class::Method`
- [ ] `Properties/launchSettings.json` criado com perfil `Lambda Test Tool` apontando para `dotnet-lambda-test-tool-10.0.exe`
- [ ] Dois arquivos de request de exemplo criados em `.lambda-test-tool/SavedRequests/` (envelope SQS e JSON direto)
- [ ] Lambda Test Tool abre sem erros via F5 no Visual Studio
- [ ] Ambos os payloads de exemplo retornam `statusCode: 200` com dados de echo válidos

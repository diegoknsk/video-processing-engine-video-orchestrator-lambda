# Video Orchestrator Lambda

Lambda .NET 10 do Video Processing Engine — orquestrador de processamento de vídeo com trigger SQS.

## Estrutura de Pastas

```
src/VideoOrchestrator/
├── Handler/           # Handler Lambda (VideoOrchestratorHandler)
├── Models/            # Modelos de domínio/DTOs (a expandir)
├── Program.cs         # Entry point (LambdaBootstrapBuilder)
└── VideoOrchestrator.csproj
```

## Pré-requisitos

- .NET 10 SDK (10.0.x)
- AWS Lambda Runtime: `dotnet10` (ou compatível)

## Build e Publish

```bash
# Restaurar dependências
dotnet restore src/VideoOrchestrator/VideoOrchestrator.csproj

# Build
dotnet build src/VideoOrchestrator/VideoOrchestrator.csproj

# Publish para Lambda (linux-x64)
dotnet publish src/VideoOrchestrator/VideoOrchestrator.csproj -c Release -r linux-x64 --self-contained false
```

Artefatos de publish em: `src/VideoOrchestrator/bin/Release/net10.0/linux-x64/publish/`

## Handler para Deploy

Ao usar `LambdaBootstrapBuilder` com `OutputType` Exe, o **handler** na AWS é o **nome do assembly**:

```
VideoOrchestrator
```

O runtime Lambda invoca o executável; o `Program.cs` inicia o bootstrap e encaminha eventos SQS para `VideoOrchestratorHandler.HandleAsync`.

## Logging

Logs estruturados com `RequestId` (AwsRequestId) e `CorrelationId` no início e fim do processamento, compatíveis com CloudWatch Logs. A resposta MOCK é registrada em log no formato JSON: `{"Status":"ok","RequestId":"...","CorrelationId":"..."}`.

---

## Pipeline de Deploy (GitHub Actions)

O workflow `.github/workflows/deploy-lambda.yml` executa em push para `main` ou `dev`:

1. **Setup .NET 10** — `actions/setup-dotnet` com versão `10.x`
2. **Restore** → **Build** → **Publish** (linux-x64)
3. **Zip** — empacota a saída do publish
4. **Deploy** — `aws lambda update-function-code` na função configurada

### Secrets necessários (Settings → Secrets and variables → Actions)

| Secret | Obrigatório | Descrição |
|--------|-------------|-----------|
| `AWS_ACCESS_KEY_ID` | Sim | Access Key da conta/role AWS |
| `AWS_SECRET_ACCESS_KEY` | Sim | Secret Key correspondente |
| `AWS_SESSION_TOKEN` | Não | Para credenciais temporárias (ex.: AssumeRole) |
| `AWS_REGION` | Sim | Região da função Lambda (ex.: `us-east-1`) |
| `AWS_LAMBDA_FUNCTION_NAME` | Sim | Nome da função Lambda a ser atualizada |

---

## Validação do Deploy

### 1. Testar invocação da função

**Console AWS:**
1. Abra o serviço Lambda → selecione a função
2. Aba **Test** → crie um evento de teste com payload SQS (ou use `test-events/sqs-test-event.json`):

```json
{
  "Records": [
    {
      "messageId": "test-msg-001",
      "receiptHandle": "test-receipt-handle",
      "body": "{\"test\": true}",
      "eventSource": "aws:sqs",
      "eventSourceARN": "arn:aws:sqs:us-east-1:123456789012:test-queue",
      "awsRegion": "us-east-1"
    }
  ]
}
```

3. Execute o teste. O retorno esperado é `SQSBatchResponse` com `batchItemFailures: []` em caso de sucesso.

**AWS CLI:**
```bash
aws lambda invoke \
  --function-name SEU_NOME_DA_FUNCAO \
  --payload '{"Records":[{"messageId":"test-001","body":"{}","eventSource":"aws:sqs","eventSourceARN":"arn:aws:sqs:us-east-1:123456789012:queue","awsRegion":"us-east-1"}]}' \
  --cli-binary-format raw-in-base64-out \
  response.json
cat response.json
```

### 2. Validar execução via CloudWatch Logs

1. **Log group:** `/aws/lambda/<nome-da-funcao>`
2. Após invocar a função, abra o log group e localize o **RequestId** da execução (visível no retorno da invocação ou no início do log stream).
3. **Campos esperados nos logs:**
   - `MOCK Response: {"Status":"ok","RequestId":"...","CorrelationId":"..."}`
   - `Início do processamento. RequestId=..., CorrelationId=...`
   - `Fim do processamento. RequestId=..., CorrelationId=..., Processados=N`

**Critérios de sucesso:**
- Invocação retorna 200 (ou sucesso no console)
- Logs contêm `Status`, `RequestId` e `CorrelationId`
- Nenhuma exceção não tratada nos logs

# Subtask-02: Adicionar step de atualização do handler na função Lambda

## Descrição
Adicionar um step no workflow que executa `aws lambda update-function-configuration` para atualizar o handler da função Lambda no AWS após cada deploy de código. Isso garante que o handler correto (`VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`) esteja sempre configurado, independente do estado anterior da função.

## Passos de Implementação

1. **Adicionar variável de ambiente `LAMBDA_HANDLER`** no bloco `env:` do workflow:
   ```yaml
   env:
     DOTNET_VERSION: '10.x'
     LAMBDA_PROJECT: src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj
     PUBLISH_OUTPUT: publish
     ZIP_NAME: lambda-deploy.zip
     LAMBDA_HANDLER: VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler
   ```

2. **Adicionar step `Update Lambda handler` logo após o step `Deploy to Lambda`:**
   ```yaml
   - name: Update Lambda handler
     run: |
       aws lambda update-function-configuration \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }} \
         --handler ${{ env.LAMBDA_HANDLER }}
   ```
   > **Importante:** `update-function-configuration` pode falhar se o código ainda está sendo atualizado. Adicionar um step de wait antes dele.

3. **Adicionar step de wait entre `Deploy to Lambda` e `Update Lambda handler`:**
   ```yaml
   - name: Wait for Lambda update to complete
     run: |
       aws lambda wait function-updated \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }}
   ```
   Esse step usa o AWS CLI waiter nativo que verifica a cada 5 segundos por até 5 minutos se a função terminou de atualizar após o `update-function-code`.

4. **Workflow final com os novos steps (ordem completa dos steps após autenticação AWS):**
   ```yaml
   - name: Deploy to Lambda
     run: |
       aws lambda update-function-code \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }} \
         --zip-file fileb://${{ env.ZIP_NAME }}

   - name: Wait for Lambda update to complete
     run: |
       aws lambda wait function-updated \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }}

   - name: Update Lambda handler
     run: |
       aws lambda update-function-configuration \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }} \
         --handler ${{ env.LAMBDA_HANDLER }}
   ```

## Formas de Teste

1. Executar o workflow via `workflow_dispatch` e verificar os logs de cada step no GitHub Actions — `Deploy to Lambda`, `Wait for Lambda update to complete` e `Update Lambda handler` devem terminar com `exit code 0`
2. Após o deploy, verificar no AWS Console (`Lambda → Functions → [function-name] → Configuration → General`) que o `Handler` está configurado como `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`
3. Invocar a função no AWS Console com o payload `{"jobId":"550e8400-e29b-41d4-a716-446655440000","correlationId":"test-001"}` e verificar resposta `statusCode: 200`

## Critérios de Aceite

- [ ] Variável `LAMBDA_HANDLER` adicionada ao bloco `env:` com o valor correto `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::Handler`
- [ ] Step `Wait for Lambda update to complete` adicionado entre o deploy de código e a atualização de configuração
- [ ] Step `Update Lambda handler` adicionado após o wait, usando `aws lambda update-function-configuration --handler`
- [ ] O workflow executa com sucesso no GitHub Actions sem erros nos novos steps
- [ ] Handler confirmado no AWS Console após deploy

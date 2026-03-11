# Subtask-01: Corrigir Path do Projeto e Steps do Workflow

## Descrição
Atualizar o workflow `.github/workflows/deploy-lambda.yml` com três correções:
1. `LAMBDA_PROJECT` apontando para o path correto do projeto após reestruturação de pastas.
2. Step de **wait** após `update-function-code` (aguardar atualização do código antes de configurar).
3. Step de **set handler** (`aws lambda update-function-configuration --handler`) seguido de mais um step de **wait**.

O path antigo (`src/VideoOrchestrator/VideoOrchestrator.csproj`) está deletado. O handler deve ser `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::FunctionHandler`.

## Passos de Implementação
1. Alterar `LAMBDA_PROJECT` na seção `env` de `src/VideoOrchestrator/VideoOrchestrator.csproj` para `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`.
2. Após o step `Deploy to Lambda`, adicionar step `Wait for code update`:
   ```yaml
   - name: Wait for code update
     run: |
       aws lambda wait function-updated \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }}
   ```
3. Adicionar step `Set Lambda handler`:
   ```yaml
   - name: Set Lambda handler
     run: |
       aws lambda update-function-configuration \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }} \
         --handler "VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::FunctionHandler"
   ```
4. Adicionar step `Wait for configuration update`:
   ```yaml
   - name: Wait for configuration update
     run: |
       aws lambda wait function-updated \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }}
       echo "Lambda function updated successfully"
   ```

## Formas de Teste
1. Executar `dotnet restore <novo-path>` localmente e confirmar que não há erros.
2. Executar `dotnet build <novo-path> -c Release` localmente e confirmar build bem-sucedido.
3. Acionar o workflow via `workflow_dispatch` no GitHub e verificar que todos os steps (incluindo wait e set handler) executam com sucesso.

## Critérios de Aceite
- [ ] `LAMBDA_PROJECT` aponta para `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`
- [ ] Step `Wait for code update` presente após o deploy de código
- [ ] Step `Set Lambda handler` presente com o handler correto: `VideoProcessing.VideoOrchestrator.Lambda::VideoProcessing.VideoOrchestrator.Lambda.Function::FunctionHandler`
- [ ] Step `Wait for configuration update` presente após o set handler
- [ ] Nenhum outro step foi alterado desnecessariamente

# Subtask-01: Atualizar variável LAMBDA_PROJECT e caminho de publish no workflow

## Descrição
Atualizar a variável de ambiente `LAMBDA_PROJECT` no arquivo `.github/workflows/deploy-lambda.yml` para apontar ao novo `.csproj` do projeto `VideoProcessing.VideoOrchestrator.Lambda`, garantindo que os steps de `restore`, `build` e `publish` usem o novo projeto corretamente.

## Passos de Implementação

1. **Localizar e atualizar a variável `LAMBDA_PROJECT`** no bloco `env:` do workflow:

   Antes:
   ```yaml
   env:
     DOTNET_VERSION: '10.x'
     LAMBDA_PROJECT: src/VideoOrchestrator/VideoOrchestrator.csproj
     PUBLISH_OUTPUT: publish
     ZIP_NAME: lambda-deploy.zip
   ```

   Depois:
   ```yaml
   env:
     DOTNET_VERSION: '10.x'
     LAMBDA_PROJECT: src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj
     PUBLISH_OUTPUT: publish
     ZIP_NAME: lambda-deploy.zip
   ```

2. **Verificar os steps que usam `LAMBDA_PROJECT`:**
   Os steps de `Restore`, `Build` e `Publish` já referenciam `${{ env.LAMBDA_PROJECT }}` via expressão — nenhuma mudança nesses steps é necessária, apenas a variável atualizada propaga automaticamente.

3. **Confirmar que o step `Publish` mantém os flags corretos:**
   ```yaml
   - name: Publish
     run: dotnet publish ${{ env.LAMBDA_PROJECT }} -c Release -r linux-x64 --self-contained false -o ${{ env.PUBLISH_OUTPUT }}
   ```
   Manter exatamente esses flags: `-r linux-x64 --self-contained false` é obrigatório para Lambda .NET 10 no runtime `provided.al2023` ou `dotnet10`.

4. **Verificar o passo de criação do ZIP** — sem alterações necessárias, pois ainda usa `${{ env.PUBLISH_OUTPUT }}`.

## Formas de Teste

1. Após a alteração, verificar o YAML com `cat .github/workflows/deploy-lambda.yml | grep LAMBDA_PROJECT` — deve mostrar o novo caminho
2. Validar a sintaxe do YAML com `python -c "import yaml; yaml.safe_load(open('.github/workflows/deploy-lambda.yml').read()); print('OK')"` ou usando ferramenta online de lint YAML
3. Confirmar no GitHub que o workflow file aparece sem erro de sintaxe na aba `Actions`

## Critérios de Aceite

- [ ] `LAMBDA_PROJECT` atualizado para `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/VideoProcessing.VideoOrchestrator.Lambda.csproj`
- [ ] Nenhum outro step do workflow foi alterado desnecessariamente nessa subtask
- [ ] O YAML é válido (sem erros de indentação ou sintaxe)
- [ ] Os 4 secrets existentes (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`, `AWS_LAMBDA_FUNCTION_NAME`) permanecem inalterados

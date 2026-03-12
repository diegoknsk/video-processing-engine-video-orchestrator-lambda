# Subtask-04: Atualizar pipeline GitHub Actions com novos secrets

## Descrição
Atualizar o workflow `deploy-lambda.yml` para injetar as novas variáveis de configuração do orquestrador como environment variables da Lambda no momento do deploy, lendo todos os valores sensíveis de GitHub Secrets.

## Passos de Implementação

1. **Adicionar os novos GitHub Secrets** ao repositório (via Settings → Secrets and variables → Actions):
   - `VIDEO_MANAGEMENT_API_BASE_URL`
   - `M2M_TOKEN_ENDPOINT`
   - `M2M_CLIENT_ID`
   - `M2M_CLIENT_SECRET`
   - `STEP_FUNCTION_ARN`
   - `VIDEO_MANAGEMENT_API_TIMEOUT_SECONDS` (pode ser variável simples, não secret — mas gerir pelo mesmo mecanismo para consistência)

2. **Atualizar o step de deploy** em `.github/workflows/deploy-lambda.yml` para incluir `--environment-variables` no comando `aws lambda update-function-configuration`:

   ```yaml
   - name: Set Lambda environment variables
     run: |
       aws lambda update-function-configuration \
         --function-name ${{ secrets.AWS_LAMBDA_FUNCTION_NAME }} \
         --environment "Variables={
           VIDEO_MANAGEMENT_API__BASE_URL=${{ secrets.VIDEO_MANAGEMENT_API_BASE_URL }},
           VIDEO_MANAGEMENT_API__TIMEOUT_SECONDS=${{ secrets.VIDEO_MANAGEMENT_API_TIMEOUT_SECONDS }},
           M2M_AUTH__TOKEN_ENDPOINT=${{ secrets.M2M_TOKEN_ENDPOINT }},
           M2M_AUTH__CLIENT_ID=${{ secrets.M2M_CLIENT_ID }},
           M2M_AUTH__CLIENT_SECRET=${{ secrets.M2M_CLIENT_SECRET }},
           STEP_FUNCTION__STATE_MACHINE_ARN=${{ secrets.STEP_FUNCTION_ARN }}
         }"
   ```
   > Nota: o duplo underscore `__` é o separador de seção no `IConfiguration` ao usar `AddEnvironmentVariables()`. Confirmar a nomenclatura com a Subtask-02.

3. **Garantir a ordem dos steps**: o step de variáveis de ambiente deve executar **após** o `wait function-updated` do deploy do código, para evitar conflito de atualização simultânea da Lambda.

## Formas de Teste

1. Executar o workflow via `workflow_dispatch` em ambiente de dev e verificar no console AWS Lambda → Configuration → Environment variables que todas as variáveis foram setadas corretamente.
2. Confirmar no log do GitHub Actions que nenhum valor sensível é impresso (os secrets devem aparecer como `***`).
3. Invocar a Lambda manualmente após o deploy e verificar nos logs do CloudWatch que a inicialização do DI (`ValidateOnStart`) não lança erros de configuração ausente.

## Critérios de Aceite
- [ ] O workflow `deploy-lambda.yml` contém o step de atualização de variáveis de ambiente, com todos os valores lidos de GitHub Secrets — sem nenhum valor hardcoded no YAML.
- [ ] O pipeline executa com sucesso em `dev` e as variáveis aparecem configuradas na Lambda no console AWS.
- [ ] Logs do CloudWatch da invocação pós-deploy não mostram erros de `OptionsValidationException` ou variável ausente.

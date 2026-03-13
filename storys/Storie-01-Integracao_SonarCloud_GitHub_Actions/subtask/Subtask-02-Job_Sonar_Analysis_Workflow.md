# Subtask-02: Adicionar job sonar-analysis e ajustar trigger no workflow

## Descrição
Modificar o workflow `.github/workflows/deploy-lambda.yml` para:
- Adicionar trigger `pull_request` para branches `main` (dispara análise SonarCloud em todo PR)
- Adicionar o job `sonar-analysis` **antes** do `build-and-deploy`
- Fazer o `build-and-deploy` depender do `sonar-analysis` via `needs`, com condição para não bloquear pushes em `dev`

**Arquivo alvo:** `.github/workflows/deploy-lambda.yml`

## Passos de implementação

1. **Adicionar trigger `pull_request`** na seção `on:` do workflow, limitado a PRs direcionados a `main`:
   ```yaml
   on:
     push:
       branches: [main, dev]
     pull_request:
       branches: [main]
     workflow_dispatch:
   ```

2. **Adicionar o job `sonar-analysis`** com as seguintes características obrigatórias:
   - `fetch-depth: 0` no checkout (obrigatório para blame info do Sonar)
   - `dotnet-version: '10.0.x'` (correspondendo ao projeto)
   - `sonar.projectBaseDir="${{ github.workspace }}"` — nunca `"."` (armadilha crítica #2)
   - `sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"`
   - Condicional `if` para rodar apenas em `pull_request` para `main` ou `push` para `main`:
   ```yaml
   sonar-analysis:
     name: SonarCloud Analysis
     runs-on: ubuntu-latest
     if: github.event_name == 'pull_request' || github.ref == 'refs/heads/main'
     steps:
       - name: Checkout code
         uses: actions/checkout@v4
         with:
           fetch-depth: 0

       - name: Setup .NET
         uses: actions/setup-dotnet@v4
         with:
           dotnet-version: '10.0.x'

       - name: Install SonarScanner
         run: dotnet tool install --global dotnet-sonarscanner

       - name: Restore dependencies
         run: dotnet restore

       - name: Begin SonarCloud analysis
         run: |
           dotnet sonarscanner begin \
             /k:"${{ vars.SONAR_PROJECT_KEY }}" \
             /o:"${{ vars.SONAR_ORGANIZATION }}" \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
             /d:sonar.host.url="https://sonarcloud.io" \
             /d:sonar.projectBaseDir="${{ github.workspace }}" \
             /d:sonar.sources="src/" \
             /d:sonar.tests="tests/" \
             /d:sonar.exclusions="**/bin/**,**/obj/**,**/*.Designer.cs" \
             /d:sonar.test.exclusions="tests/**/" \
             /d:sonar.coverage.exclusions="**/Program.cs,**/*Extensions.cs" \
             /d:sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"

       - name: Build solution
         run: dotnet build --configuration Release --no-restore

       - name: Run tests with coverage
         run: |
           dotnet test \
             --configuration Release \
             --no-build \
             --verbosity normal \
             /p:CollectCoverage=true \
             /p:CoverageReporter=opencover \
             /p:CoverletOutputFormat=opencover \
             /p:CoverletOutput=./TestResults/coverage.opencover.xml

       - name: End SonarCloud analysis
         run: |
           dotnet sonarscanner end \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
   ```

3. **Adicionar `needs` e condição ao job `build-and-deploy`** para:
   - Depender do `sonar-analysis` (quando ele rodar)
   - Só executar o deploy em eventos `push` (não em `pull_request`)
   - Usar `always()` para não bloquear push em `dev` (onde o sonar foi skipped):
   ```yaml
   build-and-deploy:
     name: Build and Deploy
     needs: [sonar-analysis]
     if: always() && (needs.sonar-analysis.result == 'success' || needs.sonar-analysis.result == 'skipped') && github.event_name != 'pull_request'
   ```

4. **Verificar indentação e sintaxe YAML** executando `cat .github/workflows/deploy-lambda.yml` ou usando um linter YAML local antes de fazer o commit.

## Formas de teste

1. **PR para `main`:** abrir um PR de qualquer branch para `main` e confirmar que o job `SonarCloud Analysis` aparece na lista de checks do PR e que o `build-and-deploy` **não** é acionado.
2. **Push para `main`:** após merge, confirmar que ambos os jobs rodam em sequência (`sonar-analysis` → `build-and-deploy`) e que os resultados aparecem no histórico de Actions.
3. **Push para `dev`:** confirmar que apenas o job `build-and-deploy` é executado (sonar é skipped) e que o deploy ocorre normalmente.

## Critérios de aceite

- [ ] Trigger `pull_request: branches: [main]` presente no workflow
- [ ] Job `sonar-analysis` com `fetch-depth: 0` e `sonar.projectBaseDir="${{ github.workspace }}"` adicionado
- [ ] Job `build-and-deploy` declara `needs: [sonar-analysis]` com condição `always()` para não bloquear pushes em `dev`
- [ ] Em PR para `main`: job `sonar-analysis` roda; `build-and-deploy` não roda
- [ ] Em push para `main`: ambos os jobs rodam em sequência
- [ ] Em push para `dev`: sonar é skipped; `build-and-deploy` executa normalmente

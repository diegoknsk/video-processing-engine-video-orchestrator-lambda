# Subtask-05: Branch Protection Rule e Badges no README

## Descrição
Configurar a Branch Protection Rule no GitHub para tornar o check `SonarCloud Analysis` obrigatório antes de merges para `main`, e adicionar badges de Quality Gate e Coverage no `README.md` do repositório.

## Passos de implementação

1. **Configurar Branch Protection Rule no GitHub:**
   - Acessar **Settings → Branches → Branch protection rules** no repositório.
   - Clicar em **Add rule** (ou editar regra existente para `main`).
   - Em **Branch name pattern**: digitar `main`.
   - Habilitar **Require status checks to pass before merging**.
   - No campo de busca de status checks, pesquisar e adicionar: `SonarCloud Analysis` (nome **exato** conforme o campo `name:` do job no workflow).
   - Habilitar **Require branches to be up to date before merging** (opcional, recomendado).
   - Salvar a regra.

2. **Ativar webhook do SonarCloud para reportar Quality Gate no PR:**
   - No SonarCloud: **Project Settings → GitHub** → ativar a integração para que o resultado do Quality Gate apareça como check no PR do GitHub.

3. **Obter as URLs dos badges do SonarCloud:**
   - No projeto SonarCloud, clicar em **Get project badges** (ícone de compartilhar ou no menu do projeto).
   - Copiar os badges no formato Markdown para:
     - **Quality Gate Status**
     - **Coverage**
   - As URLs seguem o padrão:
     ```
     [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SONAR_PROJECT_KEY&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SONAR_PROJECT_KEY)
     [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SONAR_PROJECT_KEY&metric=coverage)](https://sonarcloud.io/summary/new_code?id=SONAR_PROJECT_KEY)
     ```
   - Substituir `SONAR_PROJECT_KEY` pelo valor real da variável configurada.

4. **Adicionar os badges ao README.md:**
   - Abrir o `README.md` na raiz do repositório.
   - Inserir os dois badges logo abaixo do título principal (H1), antes de qualquer descrição:
     ```markdown
     # Video Orchestrator Lambda

     [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SEU_PROJECT_KEY&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SEU_PROJECT_KEY)
     [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SEU_PROJECT_KEY&metric=coverage)](https://sonarcloud.io/summary/new_code?id=SEU_PROJECT_KEY)
     ```

## Formas de teste

1. **Teste da Branch Protection:** tentar fazer merge de um PR para `main` **sem** o check `SonarCloud Analysis` aprovado e confirmar que o merge é bloqueado pelo GitHub.
2. **Teste dos badges:** acessar a URL de cada badge diretamente no navegador e confirmar que retorna uma imagem SVG com status (ex.: "Passed" ou "Failed"); badges funcionam após a primeira análise da branch `main`.
3. **Teste do Quality Gate no PR:** abrir um PR para `main` e confirmar que o check `SonarCloud Quality Gate` aparece na seção de checks do PR com status aprovado ou reprovado.

## Critérios de aceite

- [ ] Branch Protection Rule configurada para `main` com `SonarCloud Analysis` como check obrigatório
- [ ] Merge para `main` bloqueado quando o Quality Gate falha
- [ ] Badge de Quality Gate Status adicionado ao README.md e exibindo status correto
- [ ] Badge de Coverage adicionado ao README.md e exibindo percentual correto
- [ ] Webhook do SonarCloud reporta resultado do Quality Gate diretamente no PR do GitHub

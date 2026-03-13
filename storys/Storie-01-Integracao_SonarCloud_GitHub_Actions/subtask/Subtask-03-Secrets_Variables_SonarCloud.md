# Subtask-03: Configurar Secrets, Variables e Projeto no SonarCloud

## Descrição
Criar o projeto no SonarCloud, gerar o token de autenticação e registrar o Secret `SONAR_TOKEN` e as Variables `SONAR_PROJECT_KEY` / `SONAR_ORGANIZATION` no repositório GitHub. Desativar obrigatoriamente o Automatic Analysis no SonarCloud para evitar conflito com o CI.

## Passos de implementação

1. **Criar o projeto no SonarCloud:**
   - Acessar [sonarcloud.io](https://sonarcloud.io) e fazer login com a conta GitHub da organização.
   - Clicar em **+** (novo projeto) → selecionar o repositório `video-processing-engine-video-orchestrator-lambda`.
   - Anotar a **Project Key** (ex.: `org-video-orchestrator`) e o **Organization slug** (ex.: `fiap-video`).
   - **Atenção:** se o repositório já tiver criado um projeto automaticamente, verificar se há duplicata e deletar o projeto órfão via **Administration → Deletion**.

2. **Desativar Automatic Analysis (obrigatório — armadilha crítica):**
   - No projeto SonarCloud: **Administration → Analysis Method**.
   - Localizar o toggle **Automatic Analysis** e **desativar** (OFF).
   - Sem este passo, o pipeline falhará com: `You are running CI analysis while Automatic Analysis is enabled` (exit code 1).

3. **Gerar o SONAR_TOKEN:**
   - Acessar [sonarcloud.io/account/security](https://sonarcloud.io/account/security).
   - Clicar em **Generate Token**, dar um nome descritivo (ex.: `github-actions-video-orchestrator`) e copiar o valor — **ele não será exibido novamente**.

4. **Registrar Secret e Variables no GitHub:**

   | Tipo | Nome | Local no GitHub | Valor |
   |------|------|----------------|-------|
   | Secret | `SONAR_TOKEN` | Settings → Secrets and variables → Actions → **Secrets** | Token gerado no passo 3 |
   | Variable | `SONAR_PROJECT_KEY` | Settings → Secrets and variables → Actions → **Variables** | Project Key anotado no passo 1 |
   | Variable | `SONAR_ORGANIZATION` | Settings → Secrets and variables → Actions → **Variables** | Organization slug anotado no passo 1 |

5. **Configurar Quality Gate recomendado** no SonarCloud (**Project Settings → Quality Gate**):
   - Cobertura em novo código: **≥ 70%**
   - Sem bugs ou vulnerabilidades `CRITICAL` / `BLOCKER` em novo código
   - Maintainability Rating em novo código: **A**

## Formas de teste

1. **Verificação das variáveis:** no repositório GitHub, acessar **Settings → Secrets and variables → Actions** e confirmar que `SONAR_TOKEN` aparece em Secrets e `SONAR_PROJECT_KEY` / `SONAR_ORGANIZATION` em Variables (os valores ficam ocultos; verificar somente a existência).
2. **Primeiro pipeline:** fazer push para `main` ou abrir um PR após as configurações da Subtask-01 e Subtask-02, e verificar que o job `sonar-analysis` passa sem o erro "Automatic Analysis is enabled".
3. **Painel SonarCloud:** após o primeiro pipeline bem-sucedido, acessar o projeto no SonarCloud e confirmar que a análise foi registrada e que a cobertura de código aparece no Overview (pode exigir push para `main` para dados da branch principal).

## Critérios de aceite

- [ ] Projeto criado no SonarCloud vinculado ao repositório correto; sem projeto duplicado/órfão
- [ ] Automatic Analysis **desativada** em Administration → Analysis Method
- [ ] Secret `SONAR_TOKEN` registrado em GitHub Actions Secrets
- [ ] Variables `SONAR_PROJECT_KEY` e `SONAR_ORGANIZATION` registradas em GitHub Actions Variables
- [ ] Quality Gate configurado com cobertura mínima de 70% em novo código
- [ ] Primeiro pipeline concluído sem o erro "Automatic Analysis is enabled"

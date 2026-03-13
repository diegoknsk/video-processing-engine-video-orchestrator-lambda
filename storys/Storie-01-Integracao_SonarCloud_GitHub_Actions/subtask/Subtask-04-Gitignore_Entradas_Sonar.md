# Subtask-04: Atualizar .gitignore com entradas do Sonar

## Descrição
Adicionar ao `.gitignore` as entradas obrigatórias geradas pelo SonarScanner e pelo coverlet durante a análise local e no CI, evitando que artefatos temporários sejam rastreados pelo Git.

**Arquivo alvo:** `.gitignore` na raiz do repositório.

## Passos de implementação

1. Abrir o arquivo `.gitignore` na raiz do projeto e verificar se já existem entradas relacionadas ao Sonar ou ao coverlet.
2. Adicionar o seguinte bloco ao final do `.gitignore` (ou após a seção de artefatos .NET, se existir):
   ```gitignore
   # SonarCloud / SonarQube
   .sonarqube/
   **/.sonarqube/
   **/out/.sonar/
   .scannerwork/
   **/.scannerwork/
   coverage.opencover.xml
   ```
3. Salvar o arquivo e executar `git status` para confirmar que nenhum artefato dessas pastas aparece como arquivo a ser rastreado.

## Formas de teste

1. **Verificação via git status:** executar localmente `dotnet sonarscanner begin` (ou simular o build com testes) e confirmar com `git status` que as pastas `.sonarqube/` e `.scannerwork/` não aparecem como untracked.
2. **Verificação do arquivo:** abrir o `.gitignore` e confirmar visualmente que as 6 linhas foram adicionadas e não duplicadas.
3. **CI sem artefatos commitados:** verificar no histórico de PR que nenhum arquivo `.sonarqube/` ou `coverage.opencover.xml` aparece como adicionado ao commit.

## Critérios de aceite

- [ ] Entrada `.sonarqube/` presente no `.gitignore`
- [ ] Entrada `.scannerwork/` presente no `.gitignore`
- [ ] Entrada `coverage.opencover.xml` presente no `.gitignore`
- [ ] Entradas variantes `**/.sonarqube/` e `**/.scannerwork/` presentes para cobrir subdiretórios
- [ ] `git status` não exibe artefatos do Sonar ou coverlet como untracked após rodar análise local

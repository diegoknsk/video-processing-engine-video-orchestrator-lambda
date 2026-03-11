# Subtask 02: GitHub Action com setup-dotnet 10

## Descrição
Criar ou ajustar o workflow do GitHub Actions para usar setup-dotnet com versão 10 (10.x ou 10.0.x), garantindo que os passos de restore e build usem o SDK correto.

## Passos de implementação
1. Adicionar passo com `actions/setup-dotnet` (ou equivalente) com `dotnet-version: '10.x'` ou `10.0.x` conforme disponibilidade.
2. Garantir que os passos subsequentes (restore, build) rodem no mesmo job que usa esse setup.
3. Documentar no workflow ou em README a versão exata do .NET utilizada no CI.

## Formas de teste
1. Push ou trigger manual do workflow e verificação de que o job de build passa.
2. Inspecionar log do passo setup-dotnet e confirmar versão 10.
3. Reproduzir localmente com `dotnet --version` 10.x e comparar comportamento.

## Critérios de aceite da subtask
- [ ] Workflow utiliza setup-dotnet com versão 10 (10.x ou 10.0.x).
- [ ] Restore e build no CI usam .NET 10.
- [ ] Job de build conclui com sucesso no GitHub Actions.

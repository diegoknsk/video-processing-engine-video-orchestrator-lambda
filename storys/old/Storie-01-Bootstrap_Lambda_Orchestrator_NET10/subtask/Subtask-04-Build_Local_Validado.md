# Subtask 04: Build local validado

## Descrição
Garantir que o projeto compila e publica localmente com `dotnet build` e `dotnet publish`, com saída adequada para empacotamento em zip para Lambda (se aplicável nesta story).

## Passos de implementação
1. Executar `dotnet restore`, `dotnet build` e `dotnet publish` (com runtime e configuração típica para Lambda) na raiz do projeto.
2. Corrigir erros de compilação ou dependências até build e publish concluírem com sucesso.
3. Documentar no README os comandos exatos para build e publish local.

## Formas de teste
1. Rodar `dotnet build` e verificar saída de sucesso.
2. Rodar `dotnet publish` e verificar que a pasta de saída contém o assembly e dependências.
3. Repetir em ambiente limpo (ex.: outro diretório ou CI) para garantir reprodutibilidade.

## Critérios de aceite da subtask
- [ ] `dotnet build` conclui com sucesso.
- [ ] `dotnet publish` gera artefatos adequados para deploy Lambda.
- [ ] Comandos documentados no README ou em documento de desenvolvimento.

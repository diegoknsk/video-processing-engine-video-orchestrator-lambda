# Subtask 01: Criar projeto .NET 10 e estrutura de pastas

## Descrição
Criar o projeto de aplicação .NET 10 para o Lambda Video Orchestrator e definir a estrutura mínima de pastas (Handler, modelos, etc.) sem implementar lógica de negócio.

## Passos de implementação
1. Criar novo projeto .NET (class library ou executable conforme padrão Lambda .NET) com `<TargetFramework>net10.0</TargetFramework>` e referência ao SDK/package Lambda.
2. Definir e criar pastas mínimas: ex. `Handler/`, `Models/` (ou equivalente conforme convenção do repositório).
3. Documentar ou deixar evidente a convenção de pastas no README ou em comentário no `.csproj`.

## Formas de teste
1. Executar `dotnet build` na raiz do projeto e verificar build sem erros.
2. Verificar presença de `net10.0` no `.csproj` e de pastas criadas.
3. Validar que não há referências a AddAWSLambdaHosting ou API HTTP.

## Critérios de aceite da subtask
- [ ] Projeto compila com target .NET 10.
- [ ] Estrutura de pastas mínima criada e consistente.
- [ ] Nenhum pacote ou código de API HTTP ou Lambda Web Hosting incluído.

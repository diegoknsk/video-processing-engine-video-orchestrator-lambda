# Subtask 03: Pipeline restore, build, publish, zip e deploy Lambda

## Descrição
Implementar no GitHub Actions a sequência: restore, build, publish, geração do pacote zip para Lambda e deploy na função Lambda na AWS, com autenticação via AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY e AWS_SESSION_TOKEN (quando aplicável).

## Passos de implementação
1. Adicionar passos de `dotnet restore`, `dotnet build` e `dotnet publish` com configuração apropriada para Lambda (runtime, self-contained se necessário).
2. Adicionar passo para empacotar a saída do publish em zip (estrutura esperada pelo Lambda .NET).
3. Adicionar passo de deploy (AWS CLI update-function-code ou SDK) usando secrets do repositório para credenciais AWS (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN se for o caso).

## Formas de teste
1. Executar o workflow e verificar que não falha nos passos de publish e deploy.
2. Após o deploy, listar a função Lambda na conta e verificar última modificação/código.
3. Verificar que o zip gerado contém o handler e dependências necessárias.

## Critérios de aceite da subtask
- [ ] Pipeline executa restore, build, publish e gera zip.
- [ ] Deploy atualiza o código da função Lambda na AWS.
- [ ] Autenticação AWS usa variáveis de ambiente (secrets) conforme especificado.

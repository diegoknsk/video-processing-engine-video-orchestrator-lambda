# Subtask-02: Atualizar function-name no aws-lambda-tools-defaults.json

## Descrição
O arquivo `aws-lambda-tools-defaults.json` não possui o campo `function-name` preenchido. Adicionar o nome real da função Lambda para facilitar deploys locais via `dotnet lambda deploy-function` e servir como referência documentada do nome da função.

## Passos de Implementação
1. Confirmar o nome real da função Lambda na AWS (console AWS ou com o responsável pelo ambiente).
2. Abrir `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/aws-lambda-tools-defaults.json`.
3. Adicionar o campo `"function-name": "<nome-real-da-funcao>"` no JSON.

## Formas de Teste
1. Verificar que o JSON é válido após a edição (ex.: `dotnet lambda help` sem erros de parsing).
2. Confirmar que o nome no arquivo corresponde ao nome da função existente no console AWS.
3. (Opcional) Executar `dotnet lambda deploy-function` localmente e confirmar que o nome da função é reconhecido pela AWS.

## Critérios de Aceite
- [ ] Campo `function-name` presente e preenchido com o nome real da função em `aws-lambda-tools-defaults.json`
- [ ] Arquivo JSON continua válido (sem erros de sintaxe)
- [ ] Nome configurado corresponde ao function name real na AWS (não ao de teste do outro projeto)

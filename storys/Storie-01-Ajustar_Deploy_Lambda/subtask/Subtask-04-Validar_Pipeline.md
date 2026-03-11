# Subtask-04: Validar Pipeline End-to-End

## Descrição
Após aplicar todas as correções (path do projeto, function name no defaults e secret do GitHub), validar o pipeline completo acionando o workflow e confirmando que todos os steps executam com sucesso e a Lambda correta é atualizada na AWS.

## Passos de Implementação
1. Acionar o workflow via `workflow_dispatch` no GitHub Actions (ou fazer push em `dev`).
2. Acompanhar a execução de cada step: Checkout → Setup .NET → Restore → Build → Publish → Zip → Configure AWS → Deploy.
3. Após o deploy, verificar no console AWS (Lambda → Functions → `<nome-real>`) que o campo `Last modified` foi atualizado e o código está correto.

## Formas de Teste
1. Verificar nos logs do GitHub Actions que todos os steps passam com status verde (exit code 0).
2. No console AWS, confirmar que a função Lambda correta foi atualizada (timestamp de `Last modified` recente).
3. Invocar a Lambda manualmente no console AWS com um payload SQS de teste e verificar que responde corretamente (sem erro de runtime ou handler not found).

## Critérios de Aceite
- [ ] Todos os steps do workflow executam com sucesso (sem falhas)
- [ ] O step `Deploy to Lambda` exibe no log o nome correto da função
- [ ] A função Lambda na AWS tem o `Last modified` atualizado após o pipeline
- [ ] A função de teste do outro projeto não foi impactada

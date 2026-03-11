# Subtask-03: Atualizar Secret AWS_LAMBDA_FUNCTION_NAME no GitHub

## Descrição
O secret `AWS_LAMBDA_FUNCTION_NAME` no repositório GitHub está configurado com o nome de uma função de teste criada por outro projeto. Atualizar o secret com o nome real da função Lambda deste projeto para que o step de deploy aponte para a função correta.

## Passos de Implementação
1. Confirmar o nome real da função Lambda na AWS com o responsável pelo ambiente (nome deve corresponder ao recurso criado para este projeto — Video Orchestrator).
2. Acessar as configurações do repositório no GitHub: **Settings → Secrets and variables → Actions**.
3. Localizar o secret `AWS_LAMBDA_FUNCTION_NAME` e atualizar o valor com o nome correto da função.

## Formas de Teste
1. Após atualizar, acionar o workflow via `workflow_dispatch` e verificar o log do step `Deploy to Lambda` — o comando `aws lambda update-function-code` deve exibir o function name correto.
2. Verificar no console AWS que a Lambda correta foi atualizada (checar `Last modified` da função esperada).
3. Confirmar que a função de teste do outro projeto **não** foi afetada pelo deploy.

## Critérios de Aceite
- [ ] Secret `AWS_LAMBDA_FUNCTION_NAME` atualizado no repositório GitHub com o nome real da função deste projeto
- [ ] O step de deploy não tenta mais fazer update na função de teste do outro projeto
- [ ] A função Lambda correta na AWS é atualizada com sucesso após o deploy

# Subtask 03: Integrar verificação idempotente no fluxo do handler

## Descrição
Integrar a verificação idempotente no fluxo do handler: antes de iniciar Step Functions, chamar o update condicional; se condição falhar (já processado), considerar a mensagem processada com sucesso e não chamar StartExecution.

## Passos de implementação
1. No handler, após parsing e validação (Storie-03), obter userId e videoId e chamar o repositório de idempotência (update condicional).
2. Se o resultado for "já processado", registrar em log (correlationId, videoId) e retornar sucesso para a mensagem (deletar da fila ou responder batch com sucesso).
3. Se o resultado for "primeira vez", prosseguir para o próximo passo (Storie-05: StartExecution); nesta story pode terminar após garantir que o fluxo está correto (StartExecution será implementado na Storie-05).

## Formas de teste
1. Teste unitário do handler: mensagem nova → chama update e segue fluxo; mensagem duplicada (mock retorna já processado) → não chama StartExecution e retorna sucesso.
2. Teste de integração (opcional): duas mensagens iguais; apenas uma deve resultar em chamada ao DynamoDB com sucesso de condição.
3. Revisão: responsabilidade bem delimitada; idempotência não depende de Step Functions.

## Critérios de aceite da subtask
- [ ] Handler chama update condicional antes de iniciar execução.
- [ ] Caso "já processado", handler retorna sucesso sem iniciar Step Functions.
- [ ] Fluxo preparado para persistência de stepExecutionArn na Storie-05.

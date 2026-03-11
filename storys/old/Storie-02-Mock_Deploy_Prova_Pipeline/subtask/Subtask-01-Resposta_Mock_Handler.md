# Subtask 01: Resposta MOCK no handler

## Descrição
Alterar o handler para retornar uma resposta MOCK básica contendo status (ex.: ok), requestId do contexto Lambda e correlationId (gerado ou extraído), de forma que uma invocação de teste permita validar que o Lambda está executando.

## Passos de implementação
1. Definir estrutura de resposta MOCK (ex.: objeto ou DTO com Status, RequestId, CorrelationId).
2. No handler, preencher e retornar essa resposta usando ILambdaContext.RequestId (e gerar ou obter correlationId).
3. Garantir que o tipo de retorno seja compatível com o trigger (ex.: SQSBatchResponse para SQS) mesmo em modo mock, ou documentar que o mock é para teste antes de conectar SQS.

## Formas de teste
1. Revisão de código: resposta MOCK contém os campos definidos.
2. Invocação de teste do Lambda (console ou CLI) e verificação do payload de retorno.
3. Verificação nos logs de que requestId e correlationId aparecem.

## Critérios de aceite da subtask
- [ ] Resposta MOCK contém status (ex.: ok).
- [ ] Resposta MOCK contém requestId.
- [ ] Resposta MOCK contém correlationId (ou equivalente identificável).

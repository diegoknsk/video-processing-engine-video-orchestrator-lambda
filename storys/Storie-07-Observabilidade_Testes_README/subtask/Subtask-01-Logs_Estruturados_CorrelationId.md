# Subtask 01: Logs estruturados com correlationId em todo o fluxo

## Descrição
Garantir que todos os pontos críticos do fluxo (recebimento do batch, parsing, idempotência, StartExecution, persistência, erros) emitam logs estruturados incluindo correlationId (e requestId, videoId, stepExecutionArn quando aplicável).

## Passos de implementação
1. Definir formato de log estruturado (ex.: JSON com CorrelationId, RequestId, VideoId, Message, Exception quando houver).
2. Propagar correlationId desde o início do handler (extraído da mensagem ou gerado) para todas as chamadas de serviço e repositório que forem logar.
3. Incluir nos logs: início/fim de processamento de batch e de cada mensagem; resultado da idempotência; executionArn após StartExecution; falhas com classificação (transitório/permanente).

## Formas de teste
1. Revisão de código: todos os pontos críticos emitem log com correlationId.
2. Execução de teste (unitário ou integração) e inspeção da saída de log.
3. Após deploy: consulta no CloudWatch Logs por correlationId e verificação do rastreio completo.

## Critérios de aceite da subtask
- [ ] correlationId presente nos logs de início e fim e em eventos críticos.
- [ ] requestId, videoId e stepExecutionArn incluídos quando aplicável.
- [ ] Formato adequado para busca e filtro no CloudWatch (estruturado ou consistente).

# Subtask 03: Validação de campos obrigatórios e extração de metadados

## Descrição
Validar os campos obrigatórios após o parsing (bucket, key; e quando aplicável eventName, tamanho) e extrair metadados necessários para o Orchestrator (ex.: identificação do vídeo a partir da key ou de atributos da mensagem). Falhas de validação devem resultar em rejeição clara (para retry ou DLQ).

## Passos de implementação
1. Definir regras de validação: bucket e key obrigatórios; eventName compatível com upload (ex.: ObjectCreated:Put) se necessário.
2. Implementar validação após parsing; retornar resultado tipado (sucesso + dados ou falha + motivo).
3. Extrair metadados necessários (ex.: videoId, userId se vierem na key ou em atributos SQS/SNS) e expor no modelo interno.

## Formas de teste
1. Testes unitários: payload válido passa; payload sem bucket ou key falha com motivo claro.
2. Teste com key no formato esperado (ex.: prefix/userId/videoId/file.mp4) e verificação da extração.
3. Revisão: mensagens inválidas não lançam exceção genérica; resultado de validação é utilizável pelo handler.

## Critérios de aceite da subtask
- [ ] Validação de campos obrigatórios implementada (bucket, key e demais definidos).
- [ ] Falha de validação retorna motivo claro (sem exceção não tratada).
- [ ] Metadados necessários ao Orchestrator extraídos e expostos no modelo interno.

# Subtask 02: Modelo e parsing do evento S3

## Descrição
Definir o modelo (DTO) do evento S3 contido no Message do SNS (EventNotification ou estrutura equivalente) e implementar o parsing desse JSON para extrair bucket, key, eventName e demais campos necessários ao Orchestrator.

## Passos de implementação
1. Definir DTOs para o evento S3 (Records[].s3.bucket.name, object.key, eventName, etc.) conforme documentação AWS S3 Event Notifications.
2. Implementar deserialização do Message (SNS) como JSON para o modelo de evento S3.
3. Tratar múltiplos Records quando aplicável (definir política: primeiro record, ou falhar se mais de um para simplificar).

## Formas de teste
1. Teste unitário com JSON de evento S3 de exemplo (upload) e verificação dos campos extraídos.
2. Casos de borda: evento sem Records, Records vazio; comportamento definido (falha ou log).
3. Revisão: alinhamento com formato oficial AWS S3 Event Notifications.

## Critérios de aceite da subtask
- [ ] Modelo do evento S3 definido (bucket, key, eventName e campos necessários).
- [ ] Parsing Message SNS → evento S3 implementado.
- [ ] Comportamento definido para múltiplos Records ou formato inesperado.

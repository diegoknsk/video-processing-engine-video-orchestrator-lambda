# Subtask 01: Contrato do payload SQS e envelope SNS

## Descrição
Definir e implementar o contrato (DTOs e documentação) da mensagem SQS quando a fila recebe notificações do SNS: estrutura do body (envelope SNS com Message contendo evento S3), tipos e propriedades necessárias.

## Passos de implementação
1. Documentar o formato esperado: SQS message body = JSON do SNS (Subject, Message, Type, etc.) com Message contendo o evento S3.
2. Criar DTOs para o envelope SNS (ex.: SnsEnvelope, SnsMessage) e para o wrapper SQS (SQSEvent.SQSMessage.Body deserializado).
3. Implementar deserialização do Body da mensagem SQS para o envelope SNS (primeiro nível); tratar Message como string e preparar para segundo nível (S3).

## Formas de teste
1. Teste unitário com payload de exemplo (SQS com body SNS) e verificação da deserialização.
2. Revisão de código: tipos alinhados à documentação AWS SNS/SQS.
3. Teste com mensagem real (ou fixture) e validação dos campos extraídos.

## Critérios de aceite da subtask
- [ ] Contrato (DTOs) do payload SQS e envelope SNS definidos e documentados.
- [ ] Parsing do primeiro nível (SQS body → SNS envelope) implementado.
- [ ] Message do SNS acessível como string para parsing do evento S3.

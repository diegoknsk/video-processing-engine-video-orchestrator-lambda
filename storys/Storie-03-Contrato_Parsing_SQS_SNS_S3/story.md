# Storie-03: Contrato e parsing da mensagem SQS (SNS + S3)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como Orchestrator do Video Processing Engine, quero consumir mensagens da SQS com envelope SNS e evento S3 definidos e validados, para processar apenas eventos de upload de vídeo com payload correto e preparar o input para Step Functions.

## Objetivo
Definir e implementar o contrato do payload da mensagem SQS (envelope SNS + notificação S3), parsing robusto do envelope SNS e do evento S3, validação dos campos necessários (bucket, key, etc.) e modelo mínimo de input para Step Functions. Sem iniciar execução ainda (Storie-05).

## Escopo Técnico
- Tecnologias: .NET 10, System.Text.Json, modelos de evento SQS/SNS/S3 (AWS event shapes)
- Arquivos afetados: modelos (SqsMessage, SnsEnvelope, S3EventNotification), parser/validator, input model para Step Functions
- Componentes: DTOs de contrato, serviço ou função de parsing, modelo de input para State Machine
- Pacotes/Dependências: Amazon.Lambda.SQSEvents (ou equivalente), System.Text.Json; referência aos formatos oficiais SNS/S3

## Dependências e Riscos (para estimativa)
- Dependências: Storie-01 e Storie-02 (handler e deploy funcionando).
- Riscos: variação do formato SNS/S3 entre contas; pré-condição: documentação do formato real do evento (SNS → SQS) disponível.

## Subtasks
- [Subtask 01: Contrato do payload SQS e envelope SNS](./subtask/Subtask-01-Contrato_Payload_SQS_SNS.md)
- [Subtask 02: Modelo e parsing do evento S3](./subtask/Subtask-02-Modelo_Parsing_Evento_S3.md)
- [Subtask 03: Validação de campos obrigatórios e extração de metadados](./subtask/Subtask-03-Validacao_Extracao_Metadados.md)
- [Subtask 04: Modelo mínimo de input para Step Functions](./subtask/Subtask-04-Modelo_Input_Step_Functions.md)

## Critérios de Aceite da História
- [ ] Contrato documentado e implementado para mensagem SQS contendo envelope SNS e corpo S3
- [ ] Parsing do envelope SNS (Subscribe/Notification) e do evento S3 (Records, bucket, key, etc.) implementado
- [ ] Validação de campos obrigatórios (bucket, key, eventName quando aplicável) com falha clara para payload inválido
- [ ] Modelo mínimo de input para Step Functions definido (ex.: videoId, bucket, key, userId) sem lógica de chunks (State Machine calcula)
- [ ] Mensagens inválidas ou malformadas são rejeitadas e não causam exceção não tratada (preparação para retry/DLQ)

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

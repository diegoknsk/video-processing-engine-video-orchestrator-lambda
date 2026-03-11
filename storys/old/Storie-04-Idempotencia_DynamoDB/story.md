# Storie-04: Idempotência via DynamoDB

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como Orchestrator do Video Processing Engine, quero garantir idempotência ao processar mensagens SQS usando DynamoDB com update condicional, para evitar iniciar múltiplas execuções da State Machine para o mesmo vídeo e manter consistência com o modelo Video Management (pk=USER#userId, sk=VIDEO#videoId).

## Objetivo
Implementar idempotência robusta via DynamoDB: verificação/update condicional na tabela de vídeos (ou tabela de controle do Orchestrator) usando chave pk/sk (USER#userId, VIDEO#videoId), de forma que apenas a primeira mensagem processada para um par (userId, videoId) prossiga para StartExecution; reprocessamentos retornem sucesso sem duplicar execução.

## Escopo Técnico
- Tecnologias: .NET 10, AWS SDK for .NET (DynamoDB), tabela DynamoDB (existente do Video Management ou dedicada)
- Arquivos afetados: port/adapter DynamoDB, lógica de idempotência (condition expression), handler
- Componentes: repositório ou cliente DynamoDB, condição de update (ex.: attribute_not_exists ou status em estado inicial), modelo de item (pk, sk, status, stepExecutionArn quando aplicável)
- Pacotes/Dependências: AWSSDK.DynamoDBv2 (versão compatível .NET 10)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-03 (videoId e userId disponíveis após parsing).
- Riscos: modelo de dados do Video Management deve estar alinhado (pk/sk); pré-condição: tabela DynamoDB existente e permissões IAM do Lambda.

## Subtasks
- [Subtask 01: Definição do modelo de chave e condição de idempotência](./subtask/Subtask-01-Modelo_Chave_Condicao_Idempotencia.md)
- [Subtask 02: Implementar update condicional (ConditionExpression)](./subtask/Subtask-02-Update_Condicional_DynamoDB.md)
- [Subtask 03: Integrar verificação idempotente no fluxo do handler](./subtask/Subtask-03-Integrar_Idempotencia_Handler.md)

## Critérios de Aceite da História
- [ ] Idempotência garantida via ConditionExpression no DynamoDB (ex.: attribute_not_exists(stepExecutionArn) ou status = X)
- [ ] Chave utilizada alinhada ao Video Management: pk = USER#userId, sk = VIDEO#videoId
- [ ] Primeira mensagem para (userId, videoId) prossegue; reprocessamento retorna sucesso sem chamar StartExecution
- [ ] Persistência correta preparada para stepExecutionArn (atributo atualizado na Storie-05)
- [ ] Falha de condição (item já processado) tratada como sucesso idempotente, não como erro de negócio

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

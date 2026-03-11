# Subtask 04: Planejamento de validação de deploy e CloudWatch Logs

## Descrição
Documentar e planejar como testar o deploy do Lambda e como validar a execução via CloudWatch Logs (onde ver requestId, correlationId e resposta MOCK).

## Passos de implementação
1. Documentar no README ou em docs/ o passo a passo para validar o deploy (ex.: invocar a função via console AWS ou AWS CLI).
2. Documentar onde e como visualizar os logs no CloudWatch Logs (log group da função, campos requestId/correlationId).
3. Incluir critérios objetivos de sucesso (ex.: invocação retorna 200, logs contêm requestId e mensagem de início/fim).

## Formas de teste
1. Seguir o documento e executar uma invocação de teste; verificar resultado.
2. Abrir o log group no CloudWatch e localizar a execução pelo requestId.
3. Revisão de documentação: outro desenvolvedor consegue reproduzir a validação.

## Critérios de aceite da subtask
- [ ] Está documentado como testar o deploy (invocação da função).
- [ ] Está documentado como validar execução via CloudWatch Logs.
- [ ] Objetivo da story atendido: comprovação de que o Lambda sobe e executa.

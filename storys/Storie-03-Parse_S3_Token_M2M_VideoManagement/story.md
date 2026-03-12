# Storie-03: Parse do Evento S3, Token M2M e Integração com Video Management

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como Lambda Orquestrador, quero extrair `userId` e `videoId` da key do S3 recebida no evento, obter um token M2M via client credentials e buscar os detalhes completos do vídeo na API interna de Video Management, para que a story seguinte possa montar o payload real da Step Function com dados reais.

## Objetivo
Entregar, ao final desta story, a capacidade do orquestrador de:
1. Extrair `bucket`, `key`, `userId` e `videoId` do evento S3 recebido — a deserialização do envelope de transporte (SQS) é responsabilidade exclusiva do `Function.cs` e não pertence a esta camada.
2. Obter um token de acesso interno via OAuth2 client credentials (M2M), usando Refit + resilience.
3. Chamar `GET /internal/videos/{userId}/{videoId}` na API de Video Management com `Authorization: Bearer {token}` e mapear a resposta para um modelo de domínio.
4. Logar cada etapa e propagar exceções sem swallow — o comportamento de reprocessamento é responsabilidade da infraestrutura de mensageria, não dos UseCases.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, AWS Lambda, Refit, Microsoft.Extensions.Http.Resilience
- **Arquivos afetados/criados:**
  - `src/Core/VideoProcessing.VideoOrchestrator.Domain/Events/S3ObjectCreatedEvent.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Domain/Models/VideoDetails.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/Ports/IVideoManagementClient.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/Ports/IM2MTokenService.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/UseCases/FetchVideoDetailsUseCase.cs`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/VideoManagement/`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/ExternalApis/M2MAuth/`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/DependencyInjection.cs` (atualização)
  - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/Function.cs` (atualização do handler)
  - `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/`
- **Componentes criados/modificados:**
  - `S3ObjectCreatedEvent` — deserialização do evento S3
  - `S3KeyParser` — extração de `userId` e `videoId` da key (`videos/{userId}/{videoId}/original`)
  - `IVideoManagementApi` — interface Refit para `GET /internal/videos/{userId}/{videoId}`
  - `VideoManagementClientService` — wrapper que implementa `IVideoManagementClient`, faz chamada Refit e mapeia erros
  - `IM2MTokenService` / `M2MTokenService` — obtém token via client credentials grant
  - `FetchVideoDetailsUseCase` — orquestra parse → token → GET Video Management
- **Pacotes/Dependências:**
  - `Refit` 8.0.0 (Infra.Data)
  - `Refit.HttpClientFactory` 8.0.0 (Infra.Data)
  - `Microsoft.Extensions.Http.Resilience` 9.6.0 (Infra.Data)
  - `Amazon.Lambda.S3Events` 3.0.2 (Lambda project — para deserializar S3Event)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-02 concluída (DI bootstrapado, Options configuradas).
  - Contrato do endpoint `GET /internal/videos/{userId}/{videoId}` disponibilizado pelo time do Video Management; o formato da resposta (campos de vídeo e usuário) deve ser confirmado antes de implementar os contratos Refit.
- **Riscos/Pré-condições:**
  - A key do S3 segue **obrigatoriamente** o padrão `videos/{userId}/{videoId}/original`. Qualquer desvio deve ser tratado como erro de parse: o UseCase lança exceção, o `Function.cs` não faz swallow, e a infraestrutura de mensageria decide o reprocessamento.
  - O orquestrador **não acessa Cognito** em nenhum ponto. Nome e email do usuário são retornados integralmente pelo Video Management no endpoint de detalhes do vídeo.
  - O token M2M é obtido via endpoint HTTP de client credentials (`M2M_AUTH__TOKEN_ENDPOINT`). O orquestrador trata esse endpoint como um serviço HTTP genérico — sem acoplamento a nenhuma implementação específica de identity provider.
  - O token tem tempo de expiração; para esta story, o token é obtido a cada invocação (sem cache). Cache pode ser uma evolução futura.

## Subtasks
- [Subtask 01: Modelar evento S3 e implementar S3KeyParser](./subtask/Subtask-01-Modelo_S3Event_KeyParser.md)
- [Subtask 02: Implementar serviço de token M2M (client credentials)](./subtask/Subtask-02-Servico_Token_M2M.md)
- [Subtask 03: Implementar client Refit para API interna Video Management](./subtask/Subtask-03-Client_Refit_VideoManagement.md)
- [Subtask 04: Implementar FetchVideoDetailsUseCase e integrar no handler](./subtask/Subtask-04-UseCase_FetchVideoDetails.md)
- [Subtask 05: Testes unitários](./subtask/Subtask-05-Testes_Unitarios.md)

## Critérios de Aceite da História
- [ ] O `Function.cs` (interface layer) é responsável por deserializar o envelope de transporte e extrair a S3 key; os UseCases recebem apenas a `s3Key` como string — sem nenhuma referência a `SQSEvent` ou `SQSMessage` na camada de Application ou Domain.
- [ ] `S3KeyParser.Parse` extrai corretamente `userId` e `videoId` da key `videos/{userId}/{videoId}/original`; qualquer desvio lança exceção de domínio sem swallow.
- [ ] O token M2M é obtido via `POST` no `M2M_AUTH__TOKEN_ENDPOINT` com `client_id` e `client_secret` lidos de `IOptions<M2MAuthOptions>` — nenhuma credencial hardcoded.
- [ ] A chamada `GET /internal/videos/{userId}/{videoId}` é feita com header `Authorization: Bearer {token}`; a resposta é mapeada para o modelo de domínio `VideoDetails` contendo dados do vídeo **e** dados do usuário (email, nome) retornados pelo Video Management — sem nenhuma consulta ao Cognito.
- [ ] Erros HTTP 4xx da API Video Management (ex.: 404 vídeo não encontrado) são logados como `Error` e lançam exceção de domínio (não expõem `ApiException` para o UseCase).
- [ ] Logs estruturados com placeholders (`{UserId}`, `{VideoId}`, `{StatusCode}`) em todos os pontos críticos: início/fim do parse, solicitação do token (sem logar o secret ou o valor do token), início/fim da chamada ao Video Management.
- [ ] Testes unitários cobrem os cenários: parse de key válida, parse de key inválida, obtenção de token com sucesso, falha de token (401), busca de vídeo com sucesso, busca de vídeo 404; cobertura ≥ 80% nos componentes criados.

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

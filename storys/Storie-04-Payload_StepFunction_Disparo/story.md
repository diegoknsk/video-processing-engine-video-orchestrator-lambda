# Storie-04: Montagem do Payload e Disparo da Step Function

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como Lambda Orquestrador, quero montar o payload completo de entrada da Step Function com os dados reais do vídeo e do usuário retornados pela API de Video Management e disparar a execução da Step Function, para que o pipeline de processamento de vídeo seja iniciado de forma automatizada, rastreável e sem nenhum ARN ou dado hardcoded.

## Objetivo
Entregar, ao final desta story, o orquestrador totalmente funcional: recebe o evento S3, enriquece com dados reais (Story 03), monta o payload completo da Step Function e dispara a execução. O comportamento mockado é completamente substituído. O ARN da Step Function vem exclusivamente de `IOptions<StepFunctionOptions>`.

## Escopo Técnico
- **Tecnologias:** .NET 10, C# 13, AWS Lambda, AWS SDK for .NET (Step Functions), AWS Step Functions
- **Arquivos afetados/criados:**
  - `src/Core/VideoProcessing.VideoOrchestrator.Domain/Models/StepFunctionPayload.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/Ports/IStepFunctionService.cs`
  - `src/Core/VideoProcessing.VideoOrchestrator.Application/UseCases/OrchestrateVideoProcessingUseCase.cs`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/AwsServices/StepFunctionService.cs`
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.CrossCutting/DependencyInjection.cs` (atualização)
  - `src/InterfacesExternas/VideoProcessing.VideoOrchestrator.Lambda/Function.cs` (atualização final do handler)
  - `src/Infra/VideoProcessing.VideoOrchestrator.Infra.Data/VideoProcessing.VideoOrchestrator.Infra.Data.csproj`
  - `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/`
- **Componentes criados/modificados:**
  - `StepFunctionPayload` — modelo raiz do payload (serializado como JSON camelCase): `executionId`, `video`, `zip`, `user`
  - `VideoProcessingInput` — sub-modelo com dados do vídeo: `videoId`, `userId`, `title`, `s3Bucket`, `s3Key`, `outputPrefix` (path base de frames), `chunks` (lista de segmentos de processamento)
  - `VideoChunk` — representa um segmento de processamento: `chunkIndex`, `outputPath`
  - `ZipOutputInfo` — informações completas do zip de saída: `outputBucket`, `outputKey`
  - `UserInfo` — sub-modelo com dados do usuário (`name`, `email`) — providos exclusivamente pelo Video Management
  - `StepFunctionPayloadBuilder` — monta `StepFunctionPayload` a partir de `VideoDetails`, derivando chunks e paths de output
  - `IStepFunctionService` — porta de disparo da Step Function
  - `StepFunctionService` — implementação via `IAmazonStepFunctions` (AWS SDK)
  - `OrchestrateVideoProcessingUseCase` — UseCase final que monta o payload e dispara a Step Function
- **Pacotes/Dependências:**
  - `AWSSDK.StepFunctions` 3.7.x (Infra.Data)
  - `AWSSDK.Extensions.NETCore.Setup` 3.7.x (Infra.Data / CrossCutting)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-02 e Storie-03 concluídas — o `VideoDetails` retornado pela Story 03 é a entrada desta story.
  - O contrato de entrada da Step Function (campos esperados) deve ser definido em conjunto com o repositório de Step Functions / infra antes da implementação.
- **Riscos/Pré-condições:**
  - O payload da Step Function deve ser um JSON válido de até 256 KB (limite do AWS Step Functions para input de execução via `StartExecution`). Validar tamanho se necessário.
  - `AWSSDK.StepFunctions` usa credenciais IAM da própria Lambda (role da função). A role deve ter permissão `states:StartExecution` no ARN da Step Function — confirmar no repositório de infra.
  - O nome de execução da Step Function deve ser único por invocação; usar `videoId` (com prefixo e timestamp se necessário) como nome de execução para facilitar rastreabilidade no console.
  - Nenhum valor de ARN, nome de Step Function ou endpoint AWS deve estar hardcoded.

## Subtasks
- [Subtask 01: Definir e criar o modelo de payload da Step Function](./subtask/Subtask-01-Modelo_Payload_StepFunction.md)
- [Subtask 02: Implementar StepFunctionService com AWS SDK](./subtask/Subtask-02-StepFunctionService_AWS_SDK.md)
- [Subtask 03: Implementar OrchestrateVideoProcessingUseCase e integrar no handler](./subtask/Subtask-03-UseCase_Orquestracao_Handler.md)
- [Subtask 04: Testes unitários](./subtask/Subtask-04-Testes_Unitarios.md)

## Critérios de Aceite da História
- [ ] O handler da Lambda, ao receber um evento S3 real, percorre todo o fluxo: parse → token M2M → busca Video Management → montagem de payload → disparo da Step Function — sem nenhuma lógica mockada.
- [ ] O payload enviado à Step Function contém **obrigatoriamente** todos os campos: `executionId`, `video.videoId`, `video.userId`, `video.title`, `video.s3Bucket`, `video.s3Key`, `video.outputPrefix`, `video.chunks` (lista com ao menos um item), `zip.outputBucket`, `zip.outputKey`, `user.name` e `user.email` — ausência de qualquer campo é falha de build ou teste.
- [ ] `video.chunks` é uma lista explícita de segmentos de processamento, cada um com `chunkIndex` e `outputPath` — não está implícito no `outputPrefix` nem delegado para evolução futura.
- [ ] `zip` é um objeto separado contendo `outputBucket` e `outputKey`, representando o destino completo do arquivo zip — não uma chave genérica achatada em `video`.
- [ ] `user.name` e `user.email` vêm exclusivamente do `VideoDetails` retornado pelo Video Management — sem nenhuma consulta ao Cognito.
- [ ] O ARN da Step Function é lido exclusivamente de `IOptions<StepFunctionOptions>` — zero valores hardcoded em código ou YAML.
- [ ] O nome de execução da Step Function inclui o `videoId` para rastreabilidade e é único por invocação.
- [ ] `StepFunctionService` loga o `executionArn` retornado pela AWS após o disparo bem-sucedido.
- [ ] Falha no `StartExecution` resulta em log de erro estruturado com o motivo e propagação de exceção sem swallow — o comportamento de reprocessamento é responsabilidade da infraestrutura de mensageria.
- [ ] Testes unitários cobrem: montagem do payload verificando todos os campos obrigatórios incluindo `chunks` e `zip`, disparo bem-sucedido (mock do SDK), falha no disparo; cobertura ≥ 80% nos componentes criados nesta story.
- [ ] `dotnet build` e `dotnet test` passam no pipeline após as alterações.

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —

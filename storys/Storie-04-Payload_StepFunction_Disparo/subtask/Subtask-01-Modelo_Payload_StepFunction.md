# Subtask-01: Definir e criar o modelo de payload da Step Function

## Descrição
Definir a estrutura do payload JSON que será enviado como input da Step Function e criar os modelos de domínio correspondentes. O payload deve incluir dados reais do vídeo, a lista de chunks de processamento, as informações completas do zip de saída e os dados do usuário providos pelo Video Management.

## Passos de Implementação

1. **Definir e fixar o contrato do payload** junto ao repositório de Step Functions / infra. Os campos abaixo são **obrigatórios e não-negociáveis** — as Lambdas downstream (Video Processor, Video Finalizer) dependem de todos eles. Qualquer campo ausente deve ser tratado como falha de montagem, não como omissão silenciosa:

   ```json
   {
     "executionId": "exec-{videoId}-{unixTimestampMs}",
     "video": {
       "videoId": "16c39167-b9d5-434b-9277-29790ed46aa3",
       "userId": "0468f438-20d1-7008-aea2-a0aeeb174a5b",
       "title": "Meu Vídeo",
       "s3Bucket": "video-processing-engine-dev-videos",
       "s3Key": "videos/{userId}/{videoId}/original",
       "outputPrefix": "videos/{userId}/{videoId}/frames/",
       "chunks": [
         {
           "chunkIndex": 0,
           "outputPath": "videos/{userId}/{videoId}/frames/chunk-0/"
         }
       ]
     },
     "zip": {
       "outputBucket": "video-processing-engine-dev-videos",
       "outputKey": "videos/{userId}/{videoId}/output.zip"
     },
     "user": {
       "name": "João Silva",
       "email": "joao@example.com"
     }
   }
   ```

   Convenções fixas de paths derivados da key de entrada:
   - `video.outputPrefix` = `videos/{userId}/{videoId}/frames/` — path base dos frames extraídos
   - `video.chunks[n].outputPath` = `videos/{userId}/{videoId}/frames/chunk-{n}/` — diretório de saída do chunk `n`
   - `zip.outputBucket` = mesmo bucket do vídeo de entrada (confirmar com infra se há bucket separado)
   - `zip.outputKey` = `videos/{userId}/{videoId}/output.zip` — path do arquivo zip final

   > **Nota:** `chunks` deve conter ao menos um elemento. A estratégia de divisão em múltiplos chunks (para Map State paralelo) pode evoluir; para esta story, o orquestrador cria um único chunk cobrindo o vídeo completo. Confirmar com a equipe de infra se `zip.outputBucket` é o mesmo que `video.s3Bucket` ou um bucket dedicado.

2. **Criar os modelos de domínio** em `Domain/Models/`:

   ```csharp
   // Domain/Models/StepFunctionPayload.cs
   public record StepFunctionPayload(
       string ExecutionId,
       VideoProcessingInput Video,
       ZipOutputInfo Zip,
       UserInfo User
   );

   public record VideoProcessingInput(
       string VideoId,
       string UserId,
       string Title,
       string S3Bucket,
       string S3Key,
       string OutputPrefix,
       List<VideoChunk> Chunks
   );

   public record VideoChunk(
       int ChunkIndex,
       string OutputPath
   );

   public record ZipOutputInfo(
       string OutputBucket,
       string OutputKey
   );

   public record UserInfo(
       string Name,
       string Email
   );
   ```

3. **Criar a classe `StepFunctionPayloadBuilder`** em `Application/` responsável por montar o `StepFunctionPayload` a partir de `VideoDetails`:

   ```csharp
   public static class StepFunctionPayloadBuilder
   {
       public static StepFunctionPayload Build(VideoDetails details, string executionId)
       {
           var outputPrefix = $"videos/{details.UserId}/{details.VideoId}/frames/";

           // Chunk inicial: único segmento cobrindo o vídeo completo.
           // A divisão em múltiplos chunks é evolução futura (Map State).
           var chunks = new List<VideoChunk>
           {
               new(ChunkIndex: 0, OutputPath: $"{outputPrefix}chunk-0/")
           };

           return new StepFunctionPayload(
               ExecutionId: executionId,
               Video: new VideoProcessingInput(
                   VideoId: details.VideoId,
                   UserId: details.UserId,
                   Title: details.Title,
                   S3Bucket: details.S3Bucket,
                   S3Key: details.S3Key,
                   OutputPrefix: outputPrefix,
                   Chunks: chunks
               ),
               Zip: new ZipOutputInfo(
                   OutputBucket: details.S3Bucket,
                   OutputKey: $"videos/{details.UserId}/{details.VideoId}/output.zip"
               ),
               User: new UserInfo(
                   Name: details.User.Name,
                   Email: details.User.Email
               )
           );
       }
   }
   ```

## Formas de Teste

1. Teste unitário: `StepFunctionPayloadBuilder.Build` com `VideoDetails` de exemplo → verificar `outputPrefix`, `chunks[0].outputPath`, `zip.outputBucket` e `zip.outputKey` com asserções de string exata.
2. Teste unitário: `chunks` retornado pelo builder tem exatamente 1 item com `chunkIndex = 0` e `outputPath` seguindo o padrão `videos/{userId}/{videoId}/frames/chunk-0/`.
3. Teste unitário: serialização de `StepFunctionPayload` para JSON via `System.Text.Json` → verificar que o JSON contém todas as chaves obrigatórias em camelCase: `executionId`, `video.*`, `video.chunks[0].*`, `zip.*`, `user.*`.
4. Verificação manual: comparar o JSON serializado com o contrato definido no passo 1 e alinhar com a equipe de infra se necessário.

## Critérios de Aceite
- [ ] `StepFunctionPayload` e sub-modelos estão criados em `Domain/Models/` como records imutáveis: `StepFunctionPayload`, `VideoProcessingInput`, `VideoChunk`, `ZipOutputInfo` e `UserInfo`.
- [ ] `VideoProcessingInput` contém `Chunks: List<VideoChunk>` — não é campo opcional, não é string, não é enum.
- [ ] `StepFunctionPayloadBuilder.Build` popula `chunks` com ao menos um item; `zip.outputBucket` e `zip.outputKey` são derivados corretamente — verificado por asserções de string exata nos testes.
- [ ] A serialização JSON usa camelCase e o JSON resultante contém os campos: `executionId`, `video.videoId`, `video.userId`, `video.title`, `video.s3Bucket`, `video.s3Key`, `video.outputPrefix`, `video.chunks[0].chunkIndex`, `video.chunks[0].outputPath`, `zip.outputBucket`, `zip.outputKey`, `user.name`, `user.email` — verificado por teste unitário de serialização.
- [ ] `user.name` e `user.email` são populados a partir de `VideoDetails.User` retornado pelo Video Management — sem nenhuma consulta ao Cognito.

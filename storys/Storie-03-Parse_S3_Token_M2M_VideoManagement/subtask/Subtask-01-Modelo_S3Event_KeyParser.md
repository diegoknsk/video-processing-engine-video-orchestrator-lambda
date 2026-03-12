# Subtask-01: Modelar evento S3 e implementar S3KeyParser

## Descrição
Criar o modelo de deserialização do evento S3 e implementar a classe `S3KeyParser` responsável por extrair `userId` e `videoId` a partir da key do S3.

**Separação de responsabilidades obrigatória:**
- O modelo `S3ObjectCreatedEvent` e o `S3KeyParser` vivem em `Domain` ou `Application` e operam sobre strings puras — sem nenhuma referência a `SQSEvent`, `SQSMessage` ou qualquer tipo do SDK da Lambda.
- A responsabilidade de unwrap do envelope de transporte (deserializar o body do SQS record) é exclusiva do `Function.cs` (interface layer), que então passa apenas a `s3Key` para os UseCases.

## Passos de Implementação

1. **Criar um modelo próprio de evento S3** nos projetos `Domain` ou `Application` — não depender de `Amazon.Lambda.S3Events` para os modelos de domínio. O pacote SDK pode ser adicionado ao projeto Lambda (`InterfacesExternas`) se necessário para tipagem no `Function.cs`, mas os modelos usados pelos UseCases devem ser tipos próprios sem dependência do SDK Lambda:

   ```csharp
   // Domain/Events/S3ObjectCreatedEvent.cs
   public record S3ObjectCreatedEvent(
       [property: JsonPropertyName("Records")] List<S3Record> Records
   );

   public record S3Record(
       [property: JsonPropertyName("s3")] S3Detail S3
   );

   public record S3Detail(
       [property: JsonPropertyName("bucket")] S3Bucket Bucket,
       [property: JsonPropertyName("object")] S3Object Object
   );

   public record S3Bucket(
       [property: JsonPropertyName("name")] string Name
   );

   public record S3Object(
       [property: JsonPropertyName("key")] string Key
   );
   ```

   > Usar o modelo próprio reduz dependências e é suficiente para os campos necessários. Avaliar uso do `Amazon.Lambda.S3Events` se já utilizado em outro ponto do projeto.

2. **Criar a classe `S3KeyParser`** no projeto `Domain` ou `Application`:

   ```csharp
   // Application/Parsers/S3KeyParser.cs
   public static class S3KeyParser
   {
       // Padrão esperado: videos/{userId}/{videoId}/original
       private static readonly string ExpectedPrefix = "videos/";
       private static readonly string ExpectedSuffix = "/original";

       public static (string UserId, string VideoId) Parse(string key)
       {
           if (string.IsNullOrWhiteSpace(key))
               throw new ArgumentException("S3 key cannot be null or empty.", nameof(key));

           var withoutPrefix = key.StartsWith(ExpectedPrefix)
               ? key[ExpectedPrefix.Length..]
               : throw new FormatException($"S3 key '{key}' does not match expected prefix '{ExpectedPrefix}'.");

           var withoutSuffix = withoutPrefix.EndsWith(ExpectedSuffix)
               ? withoutPrefix[..^ExpectedSuffix.Length]
               : throw new FormatException($"S3 key '{key}' does not match expected suffix '{ExpectedSuffix}'.");

           var parts = withoutSuffix.Split('/');
           if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
               throw new FormatException($"S3 key '{key}' does not contain valid userId and videoId segments.");

           return (parts[0], parts[1]);
       }
   }
   ```

3. **No `Function.cs` (interface layer):** deserializar o payload de entrada como `S3ObjectCreatedEvent` usando `System.Text.Json` para extrair a `s3Key` e passá-la ao UseCase. O `Function.cs` é o único ponto que conhece o formato de entrega do evento — seja via SQS body, evento direto ou outro envelope. Logar o identificador da invocação e o `bucket`/`key` extraídos (nunca logar credenciais ou conteúdo do vídeo).

## Formas de Teste

1. Teste unitário de `S3KeyParser.Parse` com key válida `videos/userId-123/videoId-456/original` → retorna `("userId-123", "videoId-456")`.
2. Teste unitário com key sem prefixo, sem sufixo e com segmentos extras → verifica lançamento de `FormatException`.
3. Teste unitário de deserialização: construir JSON de evento S3 idêntico ao exemplo do prompt e verificar que o modelo mapeia corretamente `bucket.name`, `object.key`.

## Critérios de Aceite
- [ ] `S3ObjectCreatedEvent` e `S3KeyParser` não contêm nenhuma referência a tipos do SDK Lambda (`SQSEvent`, `SQSMessage`, `ILambdaContext`).
- [ ] `S3KeyParser.Parse` extrai corretamente `userId` e `videoId` da key `videos/{userId}/{videoId}/original`.
- [ ] `S3KeyParser.Parse` lança `FormatException` com mensagem descritiva para qualquer key que não siga o padrão.
- [ ] O evento S3 de exemplo do projeto (com `key = "videos/0468f438.../16c39167.../original"`) é deserializado corretamente pelo modelo criado.

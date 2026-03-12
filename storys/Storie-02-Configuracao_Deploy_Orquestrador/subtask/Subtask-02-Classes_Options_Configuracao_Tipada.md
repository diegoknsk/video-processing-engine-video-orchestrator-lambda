# Subtask-02: Criar classes Options e configuração tipada

## Descrição
Criar as classes `IOptions<T>` tipadas que representam cada grupo de configuração do orquestrador. Todas as leituras de configuração devem passar por estas classes — nenhuma parte do código deve ler `IConfiguration` diretamente fora do bootstrap.

## Passos de Implementação

1. **Criar as classes de Options** em `Infra.CrossCutting/Settings/` — **nunca em Domain**. Domain não possui dependência de configuração; Options são contratos de infraestrutura:

   ```
   VideoManagementApiOptions
     - BaseUrl: string (required)
     - TimeoutSeconds: int (default: 30)

   M2MAuthOptions
     - TokenEndpoint: string (required)
     - ClientId: string (required)
     - ClientSecret: string (required)

   StepFunctionOptions
     - StateMachineArn: string (required)
   ```

   Usar `required` para campos obrigatórios. Considerar `[Required]` + `ValidateOnStart()` para falha rápida ao inicializar a Lambda com configuração incompleta.

2. **Registrar as Options no DI** (a ser feito na Subtask-03), mas já configurar a binding no método de extensão `AddOrchestratorConfiguration(this IServiceCollection services, IConfiguration config)` dentro de `Infra.CrossCutting/DependencyInjection.cs`:

   ```csharp
   services.AddOptions<VideoManagementApiOptions>()
       .Bind(config.GetSection("VideoManagementApi"))
       .ValidateDataAnnotations()
       .ValidateOnStart();

   services.AddOptions<M2MAuthOptions>()
       .Bind(config.GetSection("M2MAuth"))
       .ValidateDataAnnotations()
       .ValidateOnStart();

   services.AddOptions<StepFunctionOptions>()
       .Bind(config.GetSection("StepFunction"))
       .ValidateDataAnnotations()
       .ValidateOnStart();
   ```

   Mapear as variáveis de ambiente para as seções acima via prefixo ou chaves customizadas (`AddEnvironmentVariables()` lê `VIDEO_MANAGEMENT_API__BASE_URL` como `VideoManagementApi:BaseUrl` com duplo underscore).

3. **Verificar o mapeamento de nomes** entre as variáveis de ambiente (SCREAMING_SNAKE_CASE com `__`) e as properties das classes Options, garantindo que a binding automática do `IConfiguration` funcione sem customização adicional.

## Formas de Teste

1. Teste unitário: construir um `IConfiguration` com valores em memória (`AddInMemoryCollection`) e verificar que a binding popula corretamente as propriedades de cada Options.
2. Teste unitário: verificar que campos `required` ausentes causam falha de validação (`OptionsValidationException`).
3. Revisão de código: confirmar que nenhuma classe de Application ou Domain acessa `IConfiguration` diretamente — somente `IOptions<T>`.

## Critérios de Aceite
- [ ] As 3 classes de Options (`VideoManagementApiOptions`, `M2MAuthOptions`, `StepFunctionOptions`) estão criadas em `Infra.CrossCutting/Settings/` — nenhuma delas em `Domain` ou `Application`.
- [ ] Nenhuma classe de `Domain` ou `Application` possui referência a `Microsoft.Extensions.Options` ou `IConfiguration`.
- [ ] O método de extensão `AddOrchestratorConfiguration` registra as 3 Options com `ValidateDataAnnotations().ValidateOnStart()`.
- [ ] Testes unitários confirmam que a binding via `AddInMemoryCollection` popula os valores corretamente e que campos ausentes causam `OptionsValidationException`.

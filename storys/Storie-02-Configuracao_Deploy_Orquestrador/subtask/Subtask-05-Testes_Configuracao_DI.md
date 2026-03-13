# Subtask-05: Testes unitários de configuração e DI

## Descrição
Criar testes unitários no projeto `VideoProcessing.VideoOrchestrator.UnitTests` para cobrir a binding de configuração, a validação das Options e a inicialização do container de DI.

## Passos de Implementação

1. **Criar classe de testes `OrchestratorConfigurationTests`** que valida a binding de cada Options via `AddInMemoryCollection`:

   ```csharp
   [Fact]
   public void VideoManagementApiOptions_WhenAllFieldsPresent_BindsCorrectly()
   {
       var config = new ConfigurationBuilder()
           .AddInMemoryCollection(new Dictionary<string, string?>
           {
               ["VideoManagementApi:BaseUrl"] = "https://api.internal",
               ["VideoManagementApi:TimeoutSeconds"] = "30"
           })
           .Build();

       var options = config.GetSection("VideoManagementApi")
                           .Get<VideoManagementApiOptions>();

       options.Should().NotBeNull();
       options!.BaseUrl.Should().Be("https://api.internal");
       options.TimeoutSeconds.Should().Be(30);
   }
   ```

2. **Criar testes de validação de campos obrigatórios** para `M2MAuthOptions` e `StepFunctionOptions`, simulando a ausência de campos obrigatórios e verificando que o `ValidateDataAnnotations()` rejeitaria a configuração:

   ```csharp
   [Fact]
   public void M2MAuthOptions_WhenClientSecretMissing_ValidationFails()
   {
       var services = new ServiceCollection();
       services.AddOptions<M2MAuthOptions>()
           .Configure(o =>
           {
               o.TokenEndpoint = "https://auth.example.com/token";
               o.ClientId = "client-id";
               // ClientSecret ausente (required)
           })
           .ValidateDataAnnotations();

       var provider = services.BuildServiceProvider();
       var monitor = provider.GetRequiredService<IOptionsMonitor<M2MAuthOptions>>();

       var act = () => monitor.CurrentValue;

       act.Should().Throw<OptionsValidationException>();
   }
   ```

3. **Verificar cobertura**: executar `dotnet test --collect:"XPlat Code Coverage"` e confirmar que os componentes criados nesta story atingem ≥ 80% de cobertura.

## Formas de Teste

1. Executar `dotnet test` e verificar que todos os testes novos passam.
2. Verificar cobertura via relatório de `coverlet` — cobertura ≥ 80% nos arquivos de Options e `DependencyInjection.cs`.
3. Revisão manual: confirmar que os testes cobrem os 3 grupos de Options e pelo menos 1 cenário de erro (campo obrigatório ausente) por grupo.

## Critérios de Aceite
- [ ] Testes para binding correta dos 3 grupos de Options (`VideoManagementApiOptions`, `M2MAuthOptions`, `StepFunctionOptions`) passam com `dotnet test`.
- [ ] Testes de validação confirmam que campos `[Required]` ausentes causam `OptionsValidationException`.
- [ ] Cobertura dos componentes criados nesta story ≥ 80%, medida por `coverlet`.

# Subtask-01: Adicionar pacotes coverlet ao projeto de testes

## Descrição
Adicionar os pacotes NuGet `coverlet.collector` e `coverlet.msbuild` ao projeto de testes unitários, configurando-os como dependências de build privadas para habilitar a geração de relatórios de cobertura no formato OpenCover.

**Arquivo alvo:** `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/VideoProcessing.VideoOrchestrator.UnitTests.csproj`

## Passos de implementação

1. Abrir o arquivo `VideoProcessing.VideoOrchestrator.UnitTests.csproj` e localizar o grupo `<ItemGroup>` de dependências de teste.
2. Adicionar as duas referências de pacote abaixo, **com os atributos `IncludeAssets` e `PrivateAssets` corretos** (impedem que os pacotes vazem para projetos dependentes):
   ```xml
   <PackageReference Include="coverlet.collector" Version="6.0.2">
     <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
     <PrivateAssets>all</PrivateAssets>
   </PackageReference>
   <PackageReference Include="coverlet.msbuild" Version="6.0.2">
     <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
     <PrivateAssets>all</PrivateAssets>
   </PackageReference>
   ```
3. Executar `dotnet restore` na raiz do projeto para confirmar que os pacotes são resolvidos sem conflito de versão.
4. Validar localmente rodando `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/coverage.opencover.xml` e verificar que o arquivo `coverage.opencover.xml` é gerado na pasta `TestResults/`.

## Formas de teste

1. **Verificação do arquivo gerado:** após executar `dotnet test` com as flags coverlet, confirmar que `src/tests/VideoProcessing.VideoOrchestrator.UnitTests/TestResults/coverage.opencover.xml` existe e não está vazio.
2. **Verificação de versão:** executar `dotnet list package` no projeto de testes e confirmar que `coverlet.collector` e `coverlet.msbuild` aparecem na versão `6.0.2`.
3. **Build sem regressão:** executar `dotnet build` na solução inteira e confirmar que nenhum erro ou warning novo é introduzido.

## Critérios de aceite

- [ ] `coverlet.collector` v6.0.2 presente no `.csproj` com `PrivateAssets=all`
- [ ] `coverlet.msbuild` v6.0.2 presente no `.csproj` com `PrivateAssets=all`
- [ ] Arquivo `coverage.opencover.xml` gerado localmente ao rodar `dotnet test` com as flags de cobertura
- [ ] `dotnet build` da solução completa passa sem erros

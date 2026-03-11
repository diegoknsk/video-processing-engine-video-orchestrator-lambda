# Subtask-01: Remover projeto existente e criar estrutura de pastas e projetos .NET

## Descrição
Remover completamente o diretório `src/VideoOrchestrator` e criar a nova estrutura de projetos `.csproj` em múltiplas camadas, seguindo o mesmo padrão do `video-processing-engine-video-management-lambda`. A solução deve ter uma `solution` (`.sln` ou `.slnx`) agrupando todos os projetos criados.

## Passos de Implementação

1. **Remover projeto existente:**
   - Deletar recursivamente `src/VideoOrchestrator/` e todos os seus arquivos
   - Verificar que não há nenhum arquivo `.csproj` residual em `src/`

2. **Criar estrutura de diretórios:**
   ```
   src/
   ├── Core/
   │   ├── VideoProcessing.VideoOrchestrator.Domain/
   │   └── VideoProcessing.VideoOrchestrator.Application/
   ├── Infra/
   │   ├── VideoProcessing.VideoOrchestrator.Infra.CrossCutting/
   │   └── VideoProcessing.VideoOrchestrator.Infra.Data/
   └── InterfacesExternas/
       └── VideoProcessing.VideoOrchestrator.Lambda/
   tests/
   └── VideoProcessing.VideoOrchestrator.UnitTests/
   ```

3. **Criar projetos .NET com `dotnet new`:**
   - `dotnet new classlib -n VideoProcessing.VideoOrchestrator.Domain -f net10.0` em `src/Core/`
   - `dotnet new classlib -n VideoProcessing.VideoOrchestrator.Application -f net10.0` em `src/Core/`
   - `dotnet new classlib -n VideoProcessing.VideoOrchestrator.Infra.CrossCutting -f net10.0` em `src/Infra/`
   - `dotnet new classlib -n VideoProcessing.VideoOrchestrator.Infra.Data -f net10.0` em `src/Infra/`
   - `dotnet new classlib -n VideoProcessing.VideoOrchestrator.Lambda -f net10.0` em `src/InterfacesExternas/`
   - `dotnet new xunit -n VideoProcessing.VideoOrchestrator.UnitTests -f net10.0` em `tests/`

4. **Criar solução e adicionar projetos:**
   - `dotnet new sln -n VideoOrchestrator` na raiz de `src/` (ou raiz do repositório)
   - `dotnet sln add` para cada `.csproj` criado

5. **Configurar referências entre projetos:**
   - `Application` → `Domain`
   - `Infra.CrossCutting` → `Application`, `Domain`
   - `Infra.Data` → `Application`, `Domain`, `Infra.CrossCutting`
   - `Lambda` → `Application`, `Domain`, `Infra.CrossCutting`, `Infra.Data`
   - `UnitTests` → `Lambda`, `Application`, `Domain`

6. **Configurar `.csproj` de cada projeto:**
   - Habilitar `Nullable`, `ImplicitUsings`, `GenerateDocumentationFile`
   - `NoWarn` para CS1591 (doc warnings)
   - No projeto `Lambda`: adicionar `<AWSProjectType>Lambda</AWSProjectType>` e `<LambdaHandler>...</LambdaHandler>`
   - No projeto `Lambda`: adicionar `<GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>`
   - No projeto `UnitTests`: adicionar pacotes `xunit`, `Moq`, `FluentAssertions`, `coverlet.collector`, `Microsoft.NET.Test.Sdk`

7. **Remover arquivos de template padrão** (`Class1.cs`, `UnitTest1.cs`) de todos os projetos

## Formas de Teste

1. `dotnet build src/VideoOrchestrator.sln` (ou equivalente) deve compilar com 0 erros
2. `dotnet sln list` deve listar todos os 6 projetos
3. Verificar que `src/VideoOrchestrator` não existe mais no sistema de arquivos

## Critérios de Aceite

- [ ] Diretório `src/VideoOrchestrator` não existe mais
- [ ] Estrutura de 5 projetos (`Domain`, `Application`, `Infra.CrossCutting`, `Infra.Data`, `Lambda`) + 1 projeto de testes criada corretamente
- [ ] Todos os projetos adicionados à solução e referências entre camadas configuradas conforme dependências acima
- [ ] `dotnet build` na solução executa sem erros (mesmo com arquivos de implementação ainda vazios)

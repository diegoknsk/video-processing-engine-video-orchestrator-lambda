using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

/// <summary>
/// Collection para testes que alteram variáveis de ambiente — execução sequencial para evitar interferência.
/// </summary>
[CollectionDefinition("EnvVars", DisableParallelization = true)]
public sealed class EnvVarsCollection;

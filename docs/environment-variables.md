# Variáveis de ambiente — Lambda Orquestrador

Este documento lista todas as variáveis de ambiente utilizadas pelo Lambda Orquestrador. O binding para `IConfiguration` usa o padrão de duplo underscore (`__`) como separador de seção (ex.: `VIDEO_MANAGEMENT_API__BASE_URL` → `VideoManagementApi:BaseUrl`).

**Uso:** valores sensíveis devem ser configurados como **GitHub Secrets** e injetados no deploy; nunca coloque secrets em texto claro no repositório ou no workflow.

| Variável de ambiente | Descrição | Obrigatória | Exemplo (não usar secrets reais) | Origem no deploy |
|----------------------|-----------|-------------|----------------------------------|-------------------|
| `VIDEO_MANAGEMENT_API__BASE_URL` | URL base da API interna de Video Management | Sim | `https://api.video-management.internal` | GitHub Secret |
| `VIDEO_MANAGEMENT_API__TIMEOUT_SECONDS` | Timeout em segundos para chamadas HTTP à API (padrão: 30) | Não | `30` | GitHub Secret ou variável |
| `M2M_AUTH__TOKEN_ENDPOINT` | URL do endpoint de token (client credentials grant) | Sim | `https://auth.example.com/oauth/token` | GitHub Secret |
| `M2M_AUTH__CLIENT_ID` | Client ID da aplicação M2M | Sim | `orchestrator-client` | GitHub Secret |
| `M2M_AUTH__CLIENT_SECRET` | Secret da aplicação M2M (**sensível**) | Sim | `<secret>` | GitHub Secret |
| `STEP_FUNCTION__STATE_MACHINE_ARN` | ARN completo da Step Function a ser disparada | Sim | `arn:aws:states:us-east-1:123456789012:stateMachine:MyStateMachine` | GitHub Secret |
| `AWS_REGION` | Região AWS (já definida pelo runtime Lambda quando aplicável) | Depende do contexto | `us-east-1` | Ambiente / GitHub Secret |

## Contrato com infraestrutura (Terraform)

O repositório de infraestrutura deve configurar a Lambda com as mesmas chaves de variáveis de ambiente. Os nomes acima (com `__` para seções) são o contrato entre este repositório e o Terraform.

## Referência de seções (binding para Options)

O provider de variáveis de ambiente usa `__` como separador; a seção no `IConfiguration` fica com o mesmo nome do prefixo (em maiúsculas):

- Seção `VIDEO_MANAGEMENT_API` ← env `VIDEO_MANAGEMENT_API__*`
- Seção `M2M_AUTH` ← env `M2M_AUTH__*`
- Seção `STEP_FUNCTION` ← env `STEP_FUNCTION__*`

# Subtask-01: Definir e documentar variáveis de configuração

## Descrição
Levantar, nomear e documentar todas as variáveis de ambiente que o Lambda Orquestrador precisará para as stories 02, 03 e 04. O resultado é uma documentação atualizada (README ou `docs/`) e um inventário que serve de contrato entre este repositório e o repositório de infra (Terraform).

## Passos de Implementação

1. **Mapear todas as configurações necessárias** a partir do escopo das 3 stories:
   - `VIDEO_MANAGEMENT_API_BASE_URL` — URL base da API interna de Video Management.
   - `VIDEO_MANAGEMENT_API_TIMEOUT_SECONDS` — Timeout em segundos para chamadas (padrão: 30).
   - `M2M_TOKEN_ENDPOINT` — URL do endpoint de token (client credentials grant).
   - `M2M_CLIENT_ID` — Client ID da aplicação M2M.
   - `M2M_CLIENT_SECRET` — Secret da aplicação M2M (**sensível**).
   - `STEP_FUNCTION_ARN` — ARN completo da Step Function a ser disparada.
   - `AWS_REGION` — Região AWS (já existe; confirmar).

2. **Criar ou atualizar o documento** `docs/environment-variables.md` com tabela contendo: nome da variável, descrição, obrigatória (S/N), valor exemplo (sem secrets reais), de onde vem (GitHub Secret ou variável de ambiente simples).

3. **Validar a nomenclatura** com o time/repositório de infra para garantir consistência (o Terraform deve setar as mesmas variáveis na configuração da Lambda).

## Formas de Teste

1. Revisão manual da tabela de variáveis: verificar que todos os campos necessários para as stories 03 e 04 estão cobertos e sem ambiguidade.
2. Confirmar que nenhuma variável sensível (secret) está listada com valor real na documentação.
3. Cross-check com o `deploy-lambda.yml` após a Subtask-04: confirmar que cada variável documentada possui seu equivalente como `env:` ou `--environment-variables` no step de deploy.

## Critérios de Aceite
- [ ] Documento `docs/environment-variables.md` criado/atualizado com todas as variáveis listadas, descritas e classificadas como obrigatórias/opcionais.
- [ ] Nenhuma variável sensível contém valor real na documentação — apenas exemplos de formato ou `<secret>`.
- [ ] A lista de variáveis é suficiente para suportar as Storys 03 e 04 sem revisão adicional desta subtask.

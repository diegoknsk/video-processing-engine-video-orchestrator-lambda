# Subtask 02: Handler puro e EntryPoint para AWS Lambda

## Descrição
Implementar a classe/função handler que a AWS Lambda invoca, com assinatura compatível com trigger SQS (ou evento genérico conforme planejamento), e configurar o entry point no assembly para o runtime Lambda .NET.

## Passos de implementação
1. Criar classe/função handler com assinatura esperada pelo Lambda (ex.: `SQSEvent` → `Task<SQSBatchResponse>` ou equivalente para .NET 10).
2. Configurar no `.csproj` ou em atributo o handler assembly (ex.: `Assembly::Namespace.Class::Method`).
3. Garantir que o handler seja invocável pelo Lambda runtime sem uso de ASP.NET Core ou AddAWSLambdaHosting.

## Formas de teste
1. Build e publish geram DLL e manifest corretos com entry point configurado.
2. Revisão de código confirma ausência de AddAWSLambdaHosting e de pipeline HTTP.
3. Documentar no README ou em comentário a string de handler usada para deploy.

## Critérios de aceite da subtask
- [ ] Handler exposto com assinatura compatível com trigger SQS (evento em batch).
- [ ] Entry point configurado corretamente para AWS Lambda.
- [ ] Nenhum uso de AddAWSLambdaHosting ou API HTTP no projeto.

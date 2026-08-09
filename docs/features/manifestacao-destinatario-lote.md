---
title: "Feature 559 — Manifestação do Destinatário em lote"
type: feature
status: implemented
created: 2026-08-05
azure_devops_id: 559
azure_devops_url: https://dev.azure.com/algoplus/ERP/_workitems/edit/559
---

# Manifestação do Destinatário em lote

## Objetivo

Adicionar ao FiscalCore uma API assíncrona para transmitir de 1 a 20 eventos de
Manifestação do Destinatário no mesmo `envEvento`, preservando o método individual.

## Tarefas e delegação

| Task | Tarefa | Executor/modelo | Effort | Justificativa |
|---|---|---|---|---|
| `#567` | Implementação do lote | Worker / `gpt-5.6-terra` | medium | Mudança concentrada na biblioteca fiscal. |
| `#569` | Compatibilidade e review | Reviewer / `gpt-5.6-sol` | medium | Validação dos consumidores e contrato público. |

## Contrato implementado

- `ManifestacaoDestinatarioServico.ManifestarLoteAsync(IReadOnlyList<ManifestacaoDestinatarioItem>, CancellationToken)`
  recebe de 1 a 20 itens e retorna `ManifestacaoDestinatarioLoteResultado`.
- `ManifestacaoDestinatarioItem`, `ManifestacaoDestinatarioResultado` e
  `ManifestacaoDestinatarioLoteResultado` são tipos públicos nomeados, com construtores
  explícitos e coleções expostas como `IReadOnlyList`.
- A transmissão cria um único `envEvento`; cada `infEvento` usa `nSeqEvento = 1`, tem
  `Id = ID + tpEvento + chNFe + 01` e é assinada antes da serialização do lote.
- A validação XSD já existente continua sendo executada quando
  `ConfiguracaoServico.ValidarXmlSchema` está habilitada; há uma única transmissão SOAP.
- Cada resultado é correlacionado por chave, tipo e sequência, e expõe `TipoEvento`,
  `cStat`, motivo, protocolo, data e
  situação semântica. `135` é `Confirmada`; `573` é `ReconciliacaoPendente`, jamais
  confirmação automática. O admin-core deve validar sua evidência local antes de concluir
  a idempotência.
- `ManifestarAsync(ChaveFiscal, eTipoEventoNFe, string, CancellationToken)` permanece
  compatível e agora adapta a chamada a um lote unitário, retornando o `retEnvEvento`
  legado.
- `xJust` é aceito exclusivamente em `210240` (`OperacaoNaoRealizada`) e deve ter de 15
  a 255 caracteres. Para os demais eventos, qualquer justificativa é rejeitada.
- O XML de retorno é salvo em `Logs/ret-eve.xml`; antes da alteração o conteúdo de retorno
  era salvo indevidamente no caminho do XML de envio.

## QA inicial

- `dotnet build FiscalCore.sln --no-restore`: aprovado.
- `dotnet test FiscalCore.sln --no-build --no-restore`: aprovado, 8 testes.
- Cobertura unitária: limites 0/1/20/21, justificativa obrigatória e exclusiva,
  resposta parcial, correlação por chave e exposição de `573` para reconciliação.

## Pendências externas

- A validação completa do XML depende da pasta de schemas configurada no consumidor e
  da opção `ValidarXmlSchema` habilitada.
- A homologação SEFAZ requer certificado A1/A3 válido e tenant habilitado; não foi
  executada neste worktree.
- A reconciliação de `573` depende da evidência persistida pelo admin-core e permanece
  propositalmente fora do FiscalCore.

## Gates

- `dotnet build FiscalCore.sln`
- testes automatizados para limites, assinatura e respostas parciais
- smoke build do `Ltec.Erp.Infrastructure` contra o commit produzido

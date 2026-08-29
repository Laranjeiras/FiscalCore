---
title: "Feature 753 — CNPJ alfanumérico e remoção de magic numbers"
type: feature
status: implemented
created: 2026-08-29
azure_devops_id: 753
azure_devops_url: https://dev.azure.com/algoplus/ERP/_workitems/edit/753
---

# CNPJ alfanumérico e remoção de magic numbers

## Objetivo

Suportar o CNPJ alfanumérico da Receita Federal (IN RFB nº 2.119/2022, vigente a
partir de julho/2026), corrigir a validação do documento do emitente — que estava
inoperante — e substituir por constantes nomeadas os comprimentos fixados pelo MOC
que apareciam como literais espalhados pelo código.

## Contexto

Uma auditoria de magic numbers no FiscalCore revelou três defeitos que não eram
apenas cosméticos:

### 1. Validação do emitente inoperante

`ConfiguracaoBasicaServico.Validar()` testava:

```csharp
CNPJEmitente.Length < 11 && CNPJEmitente.Length > 18
```

Nenhum comprimento satisfaz as duas condições ao mesmo tempo. Na prática qualquer
string não-vazia era aceita como documento do emitente.

### 2. CNPJ alfanumérico reprovado

O `struct Cnpj` percorria o valor com `char.IsDigit(c)` e descartava letras em
silêncio. Um CNPJ alfanumérico válido como `12ABC34501DE35` contabilizava apenas 8
posições em vez de 14 e era classificado como inválido.

O mesmo problema alcançava `Certificado.ExtrairCNPJArquivo`, cujo regex `\d{14}`
não reconhece letras: certificados A1 com CNPJ alfanumérico retornavam `null`.

### 3. Colisão em `eSituacaoNFe`

`UsoDenegado` e `NFeCancelada` compartilhavam o valor `3`. C# aceita a duplicação,
mas `ToString()` resolve ambos para o primeiro membro e um `switch` sobre o segundo
fica inalcançável.

## O que mudou

### Algoritmo do CNPJ (`ValueObjects/Cnpj.cs`)

Cada caractere passa a ser convertido por `ASCII − 48` (`'0'` → 0, `'A'` → 17),
conforme a tabela da IN RFB. Os pesos do módulo 11 já existentes no arquivo servem
tanto ao formato numérico quanto ao alfanumérico e foram preservados.

- Posições 1–12 aceitam `[A-Z0-9]`; posições 13–14 exigem dígitos.
- Entrada normalizada para maiúsculas e sem máscara antes da validação.
- Raiz homogênea rejeitada — a verificação cobre as 12 primeiras posições, já que
  os DVs derivam da base e mascarariam a repetição (`AAAAAAAAAAAA45`).
- CNPJs numéricos legados continuam válidos.

A API pública anterior (`Valido`, `ToString()`, `implicit operator`, `ValidarCNPJ`)
foi mantida; somaram-se `Value`, `IsValid` e `Alfanumerico`.

O buffer da normalização é dimensionado pelo comprimento de um CNPJ, não pelo da
entrada. A primeira versão usava `stackalloc char[value.Length]`: como o valor chega
de XML e de certificado, fora do controle desta camada, uma entrada suficientemente
longa alocaria centenas de KB na pilha e derrubaria o processo com
`StackOverflowException` — que não é capturável. Verificado com 5.000.000 de
caracteres.

### Validação do emitente

`Validar()` passou a delegar ao value object. Como `emit.CpfCnpj` resolve para
`CNPJ ?? CPF`, o emitente pode ser pessoa física — a validação aceita CNPJ
(inclusive alfanumérico) **ou** CPF, este último com módulo 11 próprio.

### Constantes de layout (`Tipos/LayoutFiscal.cs`)

Comprimentos e offsets fixados pelo MOC, antes literais espalhados:

| Constante | Valor |
|---|---|
| `TamanhoChaveAcesso` | 44 |
| `TamanhoProtocolo` / `TamanhoNsu` | 15 |
| `TamanhoCnpj` / `TamanhoCpf` | 14 / 11 |
| `TamanhoCnf` | 8 |
| `JustificativaMinima` / `JustificativaMaxima` | 15 / 255 |
| `CorrecaoMinima` / `CorrecaoMaxima` | 15 / 1000 |
| `MaximoEventosPorLote` | 20 |
| `TamanhoSequenciaEvento` / `TamanhoSerie` / `TamanhoNumeroNF` | 2 / 3 / 9 |

O tipo aninhado `LayoutFiscal.PosicaoChave` nomeia os offsets de parsing da chave
de acesso (campos B-01 a B-23), antes legíveis apenas contando caracteres.

### `eSituacaoNFe`

Corrigido segundo o campo C14 (`cSitNFe`), documentado em
`Modelos/DistribuicaoDFe/resNFe.cs`: `UsoAutorizado = 1`, `UsoDenegado = 2`,
`NFeCancelada = 3`. O enum não possui uso no código, então a correção não altera
comportamento existente.

## Cobertura de testes

119 testes verdes, sendo 74 adicionados por esta feature.

| Arquivo | Cobre |
|---|---|
| `ValueObjects/CnpjTests.cs` | Módulo 11 alfanumérico e numérico, máscara, caixa, DV, raiz homogênea, comprimento |
| `Configuracoes/ConfiguracaoBasicaServicoTests.cs` | Aceitação de CNPJ e CPF; rejeição efetiva de documento inválido |
| `ValueObjects/ChaveFiscalTests.cs` | Extração dos campos, round-trip de geração, comprimento |
| `Tipos/SituacaoNFeTests.cs` | Códigos do campo C14 e ausência de valores duplicados |
| `Tipos/LayoutFiscalTests.cs` | Valores do MOC; offsets contíguos cobrindo os 44 caracteres |
| `Utils/CertificadoCnpjRegexTests.cs` | Reconhecimento de CNPJ numérico e alfanumérico na extensão SAN |

## Notas de manutenção

- `.gitignore` continha `/FiscalCore.Testes/*`, que ocultava o projeto de teste
  inteiro. A regra foi estreitada para `bin/` e `obj/`; `appsettings.json` e
  `NFes/NFe_1.cs` seguem ignorados.
- As constantes `VERSAO` (`"1.00"`/`"4.00"`) nos serviços de evento foram
  mantidas como estão. Migrá-las para `eVersaoServico` exigiria trocar `const` por
  `static readonly`, já que `Descricao()` é resolvido em runtime e retorna
  `string?` — mudança de forma sem ganho de correção, dado que os valores já são
  nomeados e locais.
- Fora de escopo: coordenadas e medidas do DANFE NFC-e (`Danfe/NFCe/Nativo/`), que
  são dados de desenho sem teste de render que proteja contra regressão visual.

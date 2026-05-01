# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**FiscalCore** is a .NET class library (NuGet package) for integration with SEFAZ — Brazilian electronic fiscal documents (NF-e v4.0, NFC-e). It handles XML signing, SEFAZ SOAP communication, XML validation against XSD schemas, and DANFE generation for NFC-e.

Target framework: `netcoreapp3.1` | Published to NuGet via GitHub Actions on push to `main`.

## Build & Pack

```bash
dotnet restore
dotnet build -c Release
dotnet pack FiscalCore/FiscalCore.csproj -c Release -o out
```

No test projects exist in this solution.

## Architecture

Single library project — no Clean Architecture layers. Internal organization by concern:

| Directory | Responsibility |
|---|---|
| `Configuracoes/` | Typed config classes: certificate (A1/A3), CSC (NFCe token), emitter, service settings |
| `Modelos/` | Serializable XML models mirroring NF-e schema (NotaFiscal, Eventos, Retornos, Signatures, etc.) |
| `Servicos/` | SEFAZ service implementations — authorization, events, DFe distribution |
| `Validacoes/` | XSD schema validation + NF-e field-level business rules |
| `ValueObjects/` | `Cnpj`, `ChaveFiscal`, `ProtocoloAutorizacao`, `UrlSefaz`, `Cfop` |
| `Fabrica/` | URL factory (`FabricarUrl`) and SOAP envelope factory (`SoapEnvelopeFabrica`) |
| `Utils/` | XML serialization, certificate loading, XML signing (`Assinador`), QR code, SOAP helpers |
| `Danfe/` | Native PDF-free DANFE rendering for NFC-e using `System.Drawing.Common` |
| `Extensions/` | Extension methods: `.Assinar()` on NF-e/events, date/time helpers, enum descriptions |
| `Tipos/` | Enums: `eTipoAmbiente`, `eTipoServico`, `eUF`, `eCRT`, `eModeloDocumento`, etc. |
| `Exceptions/` | `FalhaValidacaoException`, `ConfiguracaoException`, and related fiscal exceptions |
| `Resources/` | Embedded XSD schemas loaded at runtime via `Properties/Resources.resx` |

## Key Patterns

**Template Method** — `BaseSefazServico<T>` and `BaseSefazServicoBasico<T>` define the SEFAZ request pipeline (build → sign → validate → transmit → parse response). Concrete services override individual steps.

**Policy (Strategy)** — `ValidarNFeAutorizacao` and `TratarNFeAutorizacao` apply ordered policies to NF-e objects before transmission.

**Factory** — `FabricarUrl` resolves SEFAZ endpoint URLs by `eTipoServico + eUF + eTipoAmbiente`. `SoapEnvelopeFabrica` wraps payloads into service-specific SOAP envelopes.

**Extension-based signing** — `AssinarExtension` adds `.Assinar(ConfiguracaoCertificado)` to NFe, event, and inutNFe models, encapsulating `System.Security.Cryptography.Xml` signature logic.

## Key Dependencies

- `AlgoPlus.Storage` — abstracted file/stream storage for persisting request/response XMLs
- `Microsoft.Extensions.Logging.Abstractions` — `ILogger<T>` injection in all services
- `System.Security.Cryptography.Xml` — XML digital signature (XAdES/xmldsig)
- `QRCoder` + `System.Drawing.Common` — NFC-e QR code and DANFE rendering

## SEFAZ Service Usage Pattern

```csharp
var cert = new ConfiguracaoCertificado(eTipoCertificado.A3, "thumbprint");
var cfgServico = new ConfiguracaoServico(eTipoAmbiente.Homologacao, eUF.RJ, cert, emit, csc);
cfgServico.DiretorioSchemas = @"C:\Schemas\PL_009_V4_00";

var servico = new NFeAutorizacao4(cfgServico);
var retorno = servico.Autorizar(xmlAssinado, eModeloDocumento.NFe);
```

Services are not DI-registered — they are instantiated directly with `ConfiguracaoServico`.

## Chave de Acesso Composition

`ChaveFiscal` value object composes the 44-digit access key: `cUF(2) + AAMM(4) + CNPJ(14) + mod(2) + serie(3) + nNF(9) + tpEmis(1) + cNF(8) + cDV(1)`.

## CI/CD

GitHub Actions (`.github/workflows/publish_domain.yml`) packs and pushes to NuGet on every push/PR to `main`. Requires secrets: `FISCALCORE_API_KEY`, `NUGET_SERVER`.

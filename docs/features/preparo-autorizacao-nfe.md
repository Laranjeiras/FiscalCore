---
title: "Feature 1142 — Preparo da NF-e para autorização sem transmitir"
type: feature
status: implemented
created: 2026-10-19
azure_devops_id: 1142
azure_devops_url: https://dev.azure.com/algoplus/ERP/_workitems/edit/1142
---

# Preparo da NF-e para autorização sem transmitir

## Objetivo

Permitir que o consumidor grave o XML assinado **antes** de chamar a SEFAZ (emissão resiliente do
Pyre) sem replicar nem acessar por reflection as políticas internas de autorização.

## Contrato implementado

- `AutorizarNFe4.PrepararParaEnvio(NFe)` → `NFe` assinada.
- `AutorizarNFe4.PrepararParaEnvio(IList<NFe>)` → `IList<NFe>` assinadas, na ordem recebida.

Os dois executam, nesta ordem, o mesmo preparo de `Autorizar(IList<NFe>, …)`:

1. `Certificado.Validar()` (certificado expirado → `ConfiguracaoException`);
2. `TratarNFeAutorizacao.AplicarPoliticas` (`indFinal` de não contribuinte, IPI e campos obrigatórios
   da NFC-e, destinatário e produto em homologação);
3. `ValidarNFeAutorizacao.ValidarPoliticas` (emitente igual ao configurado, modelo único no lote,
   NFC-e presencial → `FalhaValidacaoException`);
4. `ValidarEAssinar` (assinatura e, se `ValidarXmlSchema`, XSD).

`Autorizar(IList<NFe>, …)` passou a chamar `PrepararParaEnvio` e segue com o `enviNFe` e a
transmissão. `TratarNFeAutorizacao` e `ValidarNFeAutorizacao` continuam `internal`: a superfície
pública nova é só o preparo completo.

## Compatibilidade

Mudança aditiva (versão `2.1.0` → `2.2.0`). Nenhum membro público removido nem alterado;
`IAutorizarNFeServico` não muda.

## Testes

`FiscalCore.Testes/Servicos/AutorizarNFe4PrepararParaEnvioTests.cs`: políticas aplicadas na NFC-e,
NF-e preserva IPI e frete, XML idêntico ao preparo manual (políticas + validação + `ValidarEAssinar`),
lote na ordem, emitente divergente, modelos misturados e certificado expirado.

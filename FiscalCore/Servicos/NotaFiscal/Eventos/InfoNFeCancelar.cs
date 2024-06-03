using System;
using FiscalCore.ValueObjects;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class InfoNFeCancelar
    {
        public ChaveFiscal ChaveAcesso { get; private set; }
        public ProtocoloAutorizacao ProtocoloAutorizacao { get; private set; }
        public string Justificativa { get; private set; }

        public InfoNFeCancelar(ChaveFiscal chaveAcesso, ProtocoloAutorizacao protocoloAutorizacao, string justificativa = "Nota Fiscal Emitida Indevidamente")
        {
            ChaveAcesso = chaveAcesso ?? throw new ArgumentNullException(nameof(chaveAcesso));
            ProtocoloAutorizacao = protocoloAutorizacao ?? throw new ArgumentNullException(nameof(protocoloAutorizacao));
            Justificativa = justificativa;
        }
    }
}

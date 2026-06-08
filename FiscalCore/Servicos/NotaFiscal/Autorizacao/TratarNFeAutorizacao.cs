using FiscalCore.NotaFiscal;
using FiscalCore.NotaFiscal.Informacoes.Destinatario;
using FiscalCore.NotaFiscal.Informacoes.Detalhe;
using FiscalCore.Tipos;
using System.Collections.Generic;

namespace FiscalCore.Servicos
{
    internal static class TratarNFeAutorizacao
    {
        public static void AplicarPoliticas(IList<NFe> nfes)
        {
            foreach (var nfe in nfes)
            {
                AplicarPoliticas(nfe);
            }
        }

        public static void AplicarPoliticas(NFe nfe)
        {
            NaoContribuinteTemQueSerConsumidorFinal(nfe);
            NFCeNaoInformarIPI(nfe);
            NFCeDefinirCamposObrigatorios(nfe);
            TratarDestinatarioHomologacao(nfe);
        }

        public static void NaoContribuinteTemQueSerConsumidorFinal(NFe nfe)
        {
            // Não contribuinte o indFInal tem que ser o Consumidor Final
            if (nfe.infNFe.dest?.indIEDest == indIEDest.NaoContribuinte)
                nfe.infNFe.ide.indFinal = eConsumidorFinal.Sim;
        }

        public static void NFCeNaoInformarIPI(NFe nfe)
        {
            if (nfe.infNFe.ide.mod != eModeloDocumento.NFCe)
            {
                return;
            }

            foreach (var det in nfe.infNFe.det)
            {
                det.imposto.IPI = null;
            }
        }

        public static void TratarDestinatarioHomologacao(NFe nfe)
        {
            if (nfe.infNFe.ide.tpAmb == eTipoAmbiente.Homologacao)
            {
                if (nfe.infNFe?.dest != null)
                    nfe.infNFe.dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";

                var det = nfe.infNFe!.det![0];
                det.prod.xProd = "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
            }
        }

        public static void NFCeDefinirCamposObrigatorios(NFe nfe)
        {
            if (nfe.infNFe.ide.mod != eModeloDocumento.NFCe)
                return;

            nfe.infNFe.ide.dhSaiEnt = null;
            nfe.infNFe.ide.indPres = ePresencaComprador.Presencial;
            nfe.infNFe.transp.modFrete = eModalidadeFrete.SemFrete;
            nfe.infNFe.transp.veicTransp = null;
        }
    }
}

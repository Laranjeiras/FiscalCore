using FiscalCore.Configuracoes;
using FiscalCore.NotaFiscal;
using FiscalCore.NotaFiscal.Informacoes.Destinatario;
using FiscalCore.Tipos;
using System.Collections.Generic;

namespace FiscalCore.Servicos
{
    internal class TratarNFeAutorizacao
    {
        private IList<NFe> nfes;
        private ConfiguracaoServico cfgServico;

        public TratarNFeAutorizacao(ref IList<NFe> nfes, ConfiguracaoServico cfgServico)
        {
            this.nfes = nfes;
            this.cfgServico = cfgServico;
        }

        public void Tratar()
        {
            foreach (var nfe in nfes)
            {
                // Não contribuinte o indFInal tem que ser o Consumidor Final
                if (nfe.infNFe.dest?.indIEDest == indIEDest.NaoContribuinte)
                    nfe.infNFe.ide.indFinal = eConsumidorFinal.Sim;

                if (nfe.infNFe.ide.mod == eModeloDocumento.NFCe)
                {
                    foreach (var det in nfe.infNFe.det)
                    {
                        det.imposto.IPI = null;
                    }
                }

                if (nfe.infNFe.ide.tpAmb == eTipoAmbiente.Homologacao)
                {
                    if (nfe.infNFe?.dest != null)
                        nfe.infNFe.dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";

                    var det = nfe.infNFe.det[0];
                    det.prod.xProd = "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
                }
            }
        }
    }
}

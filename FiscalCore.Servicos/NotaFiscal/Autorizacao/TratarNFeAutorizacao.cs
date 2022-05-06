using FiscalCore.Configuracoes;
using FiscalCore.NotaFiscal;
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
                if (nfe.infNFe.ide.tpAmb == eTipoAmbiente.Homologacao)
                {
                    nfe.infNFe.dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
                }
            }
        }
    }
}

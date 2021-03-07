using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using DFeBR.EmissorNFe.Utilidade.Tipos;
using FiscalCore.Configuracoes;
using System.Collections.Generic;

namespace FiscalCore.Servicos
{
    public class TratarNFeAutorizacao
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
                nfe.infNFe.ide.tpAmb = TipoAmbiente.Homologacao;
                if (nfe.infNFe.ide.tpAmb == TipoAmbiente.Homologacao)
                {
                    nfe.infNFe.dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
                }
            }
        }
    }
}

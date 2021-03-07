using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Threading.Tasks;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos
{
    public class ConsultaSituacaoNFeServico
    {
        private readonly IConfiguracaoServico _cfgServico;
        private readonly string _versao;

        public ConsultaSituacaoNFeServico(IConfiguracaoServico cfgServico, string versao)
        {
            _cfgServico = cfgServico;
            _versao = versao;
        }

        public async Task<retConsSitNFe> ConsultarPelaChave(string chaveAcesso) 
        {
            chaveAcesso = chaveAcesso.Replace("NFe", "");
            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso,
                tpAmb = _cfgServico.TipoAmbiente,
                versao = _versao
            };

            var xmlEvento = Xml.ClasseParaXmlString<consSitNFe>(consSit);

            var modeloDoc = chaveAcesso.Substring(20, 2).ModeloDocumento();

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.ConsultaSituacaoNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await SefazServico.EnviarParaSefazAsync(_cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            Xml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-retConsSitNFe.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}

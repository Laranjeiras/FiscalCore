using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using FiscalCore.Fabrica;

namespace FiscalCore.Servicos
{
    public class ConsultaSituacaoNFeServico
    {
        private readonly ConfiguracaoServico _cfgServico;
        private readonly ITransmitirSefazCommand sefaz;
        private readonly string _versao;

        public ConsultaSituacaoNFeServico(ConfiguracaoServico cfgServico, ITransmitirSefazCommand transmitir)
        {
            _cfgServico = cfgServico;
            this.sefaz = transmitir;
            _versao = "0.00";
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

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            var modeloDoc = chaveAcesso.Substring(20, 2).ModeloDocumento();

            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ConsultaSituacaoNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await sefaz.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            await Arquivo.SalvarArquivoAsync(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-retConsSitNFe.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}

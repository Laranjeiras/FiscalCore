using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;

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

        public retConsSitNFe ConsultarPelaChave(string chaveAcesso) 
        {
            chaveAcesso = chaveAcesso.Replace("NFe", "");
            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso,
                tpAmb = _cfgServico.TipoAmbiente,
                versao = _versao
            };

            var xmlEvento = consSit.ObterXmlString();

            var modeloDoc = Conversor.ModeloDocumento(chaveAcesso.Substring(20, 2));

            var sefazUrl = ObterSefazUrl.ObterUrl(fcServico.ConsultaSituacaoNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = SoapEnvelopes.FabricarEnvelopeConsultarSituacaoNFe(xmlEvento);

            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.ClearEnvelop(retornoXmlString, "retConsSitNFe").OuterXml;

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-retConsSitNFe.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}

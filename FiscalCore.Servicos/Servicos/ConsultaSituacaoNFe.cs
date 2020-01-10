using FiscalCore.Main.Configuracoes;
using FiscalCore.Main.Enums;
using FiscalCore.Main.Extensions;
using FiscalCore.Main.Models.Consulta;
using FiscalCore.Main.Models.Retornos;
using FiscalCore.Main.Utils;
using FiscalCore.Servicos.Utils;
using System;

namespace FiscalCore.Servicos.Servicos
{
    public class ConsultaSituacaoNFe
    {
        private readonly ConfiguracaoServico _cfgServico;
        private readonly string _versao;

        public ConsultaSituacaoNFe(ConfiguracaoServico cfgServico, string versao)
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

            var modeloDoc = ModeloDocumento(chaveAcesso.Substring(20, 2));

            var sefazUrl = ObterSefazUrl.ObterUrl(fcServico.ConsultaSituacaoNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = SoapEnvelopes.FabricarEnvelopeConsultarSituacaoNFe(xmlEvento);

            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, modeloDoc, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.ClearEnvelop(retornoXmlString, "retConsSitNFe").OuterXml;

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-retConsSitNFe.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }

        private static eModeloDocumento ModeloDocumento(string modelo)
        {
            if (modelo != "55" && modelo != "65")
                throw new Exception("Os modelos válidos são 55 ou 65");
            var modeloDoc = (eModeloDocumento)Enum.Parse(typeof(eModeloDocumento), modelo);
            return modeloDoc;
        }
    }
}

using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;
using Zion.Common2.Helpers;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4Servico
    {
        ConfiguracaoServico _cfgServico;

        public AutorizarNFe4Servico(ConfiguracaoServico cfgServico)
        {
            _cfgServico = cfgServico;
        }

        public RetNFeAutorizacao4 Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento)
        {
            var versaoServico = EnumHelper.GetDescription(_cfgServico.VersaoAutorizacaoNFe);

            FuncoesXml.SalvarArquivoXml($"{_cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-env-nfe.xml", xmlenviNFe4);

            var urlSefaz = ObterSefazUrl.ObterUrl(fcServico.AutorizarNFe, _cfgServico.TipoAmbiente, modeloDocumento, _cfgServico.UF);

            var envelope = SoapEnvelopes.FabricarEnvelopeAutorizacaoNFe4(xmlenviNFe4);

            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, urlSefaz, envelope);
            var retornoLimpo = Soap.ClearEnvelop(retornoXmlString, "retEnviNFe").OuterXml;

            FuncoesXml.SalvarArquivoXml($"{_cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-ret-env-nfe.xml", retornoLimpo);

            return new RetNFeAutorizacao4(retornoLimpo);
        }
    }
}

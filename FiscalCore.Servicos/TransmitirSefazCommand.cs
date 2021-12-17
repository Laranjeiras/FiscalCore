using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace FiscalCore.Servicos
{
    public class TransmitirSefazCommand : ITransmitirSefazCommand
    {
        private readonly ConfiguracaoServico configuracao;
        private readonly IStorage storage;
        private readonly ILogger<TransmitirSefazCommand> logger;

        public TransmitirSefazCommand(ConfiguracaoServico configuracao, IStorage storage, ILogger<TransmitirSefazCommand> logger)
        {
            this.configuracao = configuracao;
            this.storage = storage;
            this.logger = logger;
        }

        public virtual async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            HttpWebRequest webRequest = SoapEnvelopeFabrica.CriarWebRequest(sefazUrl.Url, "application/soap+xml;charset=utf-8");

            Soap.InserirSoapEnvelopeWebRequest(envelope, webRequest);

            webRequest.ClientCertificates.Add(ObterCertificado.Obter(configuracao.ConfigCertificado));

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using StreamReader rd = new StreamReader(webResponse.GetResponseStream());
                    soapResult = await rd.ReadToEndAsync();
            }

            return soapResult;
        }

        public virtual async Task<string> TransmitirAsync(distDFeInt distDFeInt, bool validarXmlConsulta = true)
        {
            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (validarXmlConsulta)
                Schemas.ValidarSchema(eTipoServico.NFeDistribuicaoDFe, xml, configuracao);

            var arqEnv = Path.Combine("Logs", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml");
            var stRet = await storage.SaveAsync(arqEnv, xml);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);

            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.NFeDistribuicaoDFe, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var retorno = await TransmitirAsync(sefazUrl, envelope);

            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;

            var arqRet = Path.Combine("Logs", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-retDistDFeInt.xml");
            await storage.SaveAsync(arqRet, retornoLimpo);

            return retornoLimpo;
        }
    }
}

using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Utils;
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
        private readonly ConfiguracaoBasicaServico configuracao;
        private readonly ILogger logger;

        public TransmitirSefazCommand(ConfiguracaoBasicaServico configuracao, ILogger<TransmitirSefazCommand> logger = null)
        {
            this.configuracao = configuracao;
            this.logger = logger;
        }

        public virtual async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            logger?.LogInformation($"INICIANDO TRANSMISSÃO SEFAZ [{sefazUrl.Url}]");

            HttpWebRequest webRequest = SoapEnvelopeFabrica.CriarWebRequest(sefazUrl.Url, "application/soap+xml;charset=utf-8");

            Soap.InserirSoapEnvelopeWebRequest(envelope, webRequest);

            webRequest.ClientCertificates.Add(configuracao.ConfigCertificado.Certificado);

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using StreamReader rd = new StreamReader(webResponse.GetResponseStream());
                    soapResult = await rd.ReadToEndAsync();
            }

            logger?.LogInformation($"ENCERRANDO TRANSMISSÃO SEFAZ");

            return soapResult;
        }
    }
}

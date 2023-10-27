using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

namespace FiscalCore.Servicos
{
    public class TransmitirSefazCommand : ITransmitirSefazCommand
    {
        private readonly ConfiguracaoBasicaServico configuracao;
        private readonly ILogger<TransmitirSefazCommand> logger;

        public TransmitirSefazCommand(ConfiguracaoBasicaServico configuracao, ILogger<TransmitirSefazCommand> logger = null)
        {
            this.configuracao = configuracao;
            this.logger = logger;
        }

        public virtual async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            logger?.LogInformation($"INICIANDO TRANSMISSÃO SEFAZ [{sefazUrl.Url}]");

            var certificado = CarregarCertificado();

            var webRequest = SoapEnvelopeFabrica.CriarWebRequest(sefazUrl.Url, certificado);

            Soap.InserirSoapEnvelopeWebRequest(envelope, webRequest);

            var soapResult = await Transmitir(webRequest);

            return soapResult;
        }

        private async Task<string> Transmitir(HttpWebRequest webRequest)
        {
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            logger?.LogDebug("TRANSMITINDO...");

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using StreamReader rd = new StreamReader(webResponse.GetResponseStream());
                soapResult = await rd.ReadToEndAsync();
            }

            logger?.LogInformation($"ENCERRANDO TRANSMISSÃO SEFAZ");

            return soapResult;
        }

        private X509Certificate2 CarregarCertificado()
        {
            logger?.LogDebug("CARREGANDO INFORMAÇÕES DO CERTIFICADO");

            var certificado = configuracao?.ConfigCertificado?.Certificado
                ?? throw new ArgumentNullException("NÁO FOI POSSÍVEL CARREGAR CONFIGURAÇÕES DO CERTIFICADO");

            logger?.LogDebug("INFORMAÇÕES DO CERTIFICADO CARREGADAS");

            return certificado;
        }
    }
}
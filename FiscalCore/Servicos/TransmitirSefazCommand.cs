using FiscalCore.Configuracoes;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FiscalCore.Servicos
{
    public class TransmitirSefazCommand : ITransmitirSefazCommand
    {
        private readonly ConfiguracaoBasicaServico configuracao;
        private readonly ILogger<TransmitirSefazCommand>? logger;

        public TransmitirSefazCommand(ConfiguracaoBasicaServico configuracao, ILogger<TransmitirSefazCommand>? logger = null)
        {
            this.configuracao = configuracao;
            this.logger = logger;
        }

        public virtual async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            logger?.LogDebug("INICIANDO TRANSMISSÃO SEFAZ [{Url}]", sefazUrl.Url);

            TemCertificado(configuracao);
            var certificado = CarregarCertificado();

            logger?.LogDebug("TRANSMITINDO...");
            var soapResult = await EnviarSoapAsync(sefazUrl.Url, envelope, certificado);
            logger?.LogDebug("ENCERRANDO TRANSMISSÃO SEFAZ");

            return soapResult;
        }

        private static void TemCertificado(ConfiguracaoBasicaServico configuracao)
        {
            if (configuracao?.ConfigCertificado?.Certificado == null)
                throw new ArgumentNullException(nameof(configuracao), "NÃO FOI POSSÍVEL CARREGAR CONFIGURAÇÕES DO CERTIFICADO");
        }

        private static async Task<string> EnviarSoapAsync(string url, XmlDocument envelope, X509Certificate2 certificado)
        {
            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            handler.ClientCertificates.Add(certificado);

            using var client = new HttpClient(handler);

            envelope.PreserveWhitespace = true;
            var content = new StringContent(envelope.OuterXml, Encoding.UTF8, "application/soap+xml");

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            request.Headers.TryAddWithoutValidation("SOAP:Action", string.Empty);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private X509Certificate2 CarregarCertificado()
        {
            logger?.LogDebug("CARREGANDO INFORMAÇÕES DO CERTIFICADO");

            var certificado = configuracao?.ConfigCertificado?.Certificado
                ?? throw new ArgumentNullException(nameof(configuracao), "NÃO FOI POSSÍVEL CARREGAR CONFIGURAÇÕES DO CERTIFICADO");

            logger?.LogDebug("INFORMAÇÕES DO CERTIFICADO CARREGADAS");

            return certificado;
        }
    }
}
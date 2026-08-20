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
using System.Threading;
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
            return await TransmitirAsync(sefazUrl, envelope, CancellationToken.None).ConfigureAwait(false);
        }

        public virtual async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope, CancellationToken cancellation)
        {
            logger?.LogDebug("INICIANDO TRANSMISSÃO SEFAZ [{Url}]", sefazUrl.Url);

            TemCertificado(configuracao);
            var certificado = CarregarCertificado();

            logger?.LogDebug("TRANSMITINDO...");
            var soapResult = await EnviarSoapAsync(sefazUrl.Url, envelope, certificado, cancellation).ConfigureAwait(false);
            logger?.LogDebug("ENCERRANDO TRANSMISSÃO SEFAZ");

            return soapResult;
        }

        private static void TemCertificado(ConfiguracaoBasicaServico configuracao)
        {
            if (configuracao?.ConfigCertificado?.Certificado == null)
                throw new ArgumentNullException(nameof(configuracao), "NÃO FOI POSSÍVEL CARREGAR CONFIGURAÇÕES DO CERTIFICADO");
        }

        protected virtual HttpMessageHandler CriarHttpMessageHandler(X509Certificate2 certificado)
        {
            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            handler.ClientCertificates.Add(certificado);
            return handler;
        }

        protected virtual async Task<string> EnviarSoapAsync(string url, XmlDocument envelope, X509Certificate2 certificado, CancellationToken cancellation)
        {
            using var client = new HttpClient(CriarHttpMessageHandler(certificado));

            envelope.PreserveWhitespace = true;
            var content = new StringContent(envelope.OuterXml, Encoding.UTF8, "application/soap+xml");

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            request.Headers.TryAddWithoutValidation("SOAP:Action", string.Empty);

            var response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
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

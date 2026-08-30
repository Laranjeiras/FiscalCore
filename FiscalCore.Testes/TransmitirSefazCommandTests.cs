using FiscalCore.Configuracoes;
using FiscalCore.Servicos;
using FiscalCore.Servicos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace FiscalCore.Testes;

public sealed class TransmitirSefazCommandTests
{
    [Fact]
    public async Task FluxoMde_ComSubclasseLegadaSobrescrevendoDoisArgumentos_DeveUsarASobrescrita()
    {
        // Arrange
        var transmissor = new TransmissorLegado();
        var envelope = new XmlDocument();
        envelope.LoadXml("<Envelope />");
        var url = new UrlSefaz(
            eTipoServico.ManifestacaoDestinatario,
            eUF.AN,
            eTipoAmbiente.Homologacao,
            eModeloDocumento.NFe,
            "https://sefaz.example.test/recepcao",
            string.Empty);

        // Act
        var retorno = await ManifestacaoDestinatarioServico.TransmitirComRetryAntesDoEnvioAsync(
            transmissor,
            NullLogger.Instance,
            url,
            envelope,
            CancellationToken.None);

        // Assert
        Assert.Equal("<retorno-legado />", retorno);
        Assert.Equal(1, transmissor.ChamadasLegadas);
    }

    [Fact]
    public async Task TransmitirAsync_ComSobrescritaLegadaChamandoBase_DeveExecutarNucleoUmaVezSemRecursao()
    {
        // Arrange
        var transmissor = new TransmissorLegadoQueDelegaParaBase();
        var envelope = new XmlDocument();
        envelope.LoadXml("<Envelope />");
        var url = new UrlSefaz(
            eTipoServico.ManifestacaoDestinatario,
            eUF.AN,
            eTipoAmbiente.Homologacao,
            eModeloDocumento.NFe,
            "https://sefaz.example.test/recepcao",
            string.Empty);

        // Act
        var retorno = await transmissor.TransmitirAsync(url, envelope, CancellationToken.None);

        // Assert
        Assert.Equal("<retorno-core />", retorno);
        Assert.Equal(1, transmissor.ChamadasLegadas);
        Assert.Equal(1, transmissor.ChamadasCore);
    }

    [Fact]
    public async Task EnviarSoapAsync_ComCancellationTokenInformado_DeveRepassarAoHttpHandler()
    {
        // Arrange
        var handler = new CapturadorHttpMessageHandler();
        var transmissor = new TransmissorComHandler(handler);
        var envelope = new XmlDocument();
        envelope.LoadXml("<Envelope />");
        using var cancellation = new CancellationTokenSource();

        // Act
        var retorno = await transmissor.EnviarAsync(envelope, cancellation.Token);

        // Assert
        Assert.Equal("<ok />", retorno);
        Assert.True(handler.CancellationTokenRecebido.CanBeCanceled);
        Assert.Equal("<Envelope />", handler.CorpoRecebido);
    }

    private sealed class TransmissorComHandler : TransmitirSefazCommand
    {
        private readonly HttpMessageHandler handler;

        public TransmissorComHandler(HttpMessageHandler handler)
            : base(new ConfiguracaoBasicaServico())
        {
            this.handler = handler;
        }

        public Task<string> EnviarAsync(XmlDocument envelope, CancellationToken cancellationToken) =>
            EnviarSoapAsync("https://sefaz.example.test/recepcao", envelope, null!, cancellationToken);

        protected override HttpMessageHandler CriarHttpMessageHandler(X509Certificate2 certificado) => handler;
    }

    private sealed class TransmissorLegado : TransmitirSefazCommand
    {
        public TransmissorLegado()
            : base(new ConfiguracaoBasicaServico())
        {
        }

        public int ChamadasLegadas { get; private set; }

        public override Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            ChamadasLegadas++;
            return Task.FromResult("<retorno-legado />");
        }
    }

    private sealed class TransmissorLegadoQueDelegaParaBase : TransmitirSefazCommand
    {
        public TransmissorLegadoQueDelegaParaBase()
            : base(new ConfiguracaoBasicaServico())
        {
        }

        public int ChamadasLegadas { get; private set; }
        public int ChamadasCore { get; private set; }

        public override async Task<string> TransmitirAsync(UrlSefaz sefazUrl, XmlDocument envelope)
        {
            ChamadasLegadas++;
            return await base.TransmitirAsync(sefazUrl, envelope);
        }

        protected override Task<string> TransmitirInternamenteAsync(UrlSefaz sefazUrl, XmlDocument envelope, CancellationToken cancellation)
        {
            ChamadasCore++;
            return Task.FromResult("<retorno-core />");
        }
    }

    private sealed class CapturadorHttpMessageHandler : HttpMessageHandler
    {
        public CancellationToken CancellationTokenRecebido { get; private set; }
        public string? CorpoRecebido { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CancellationTokenRecebido = cancellationToken;
            CorpoRecebido = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<ok />")
            };
        }
    }
}

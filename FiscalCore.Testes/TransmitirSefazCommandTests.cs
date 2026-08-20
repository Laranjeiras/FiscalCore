using FiscalCore.Configuracoes;
using FiscalCore.Servicos;
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
    public async Task EnviarSoapAsync_WhenCancellationProvided_PassesItToHttpHandler()
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

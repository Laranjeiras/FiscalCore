using FiscalCore.ValueObjects;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;

namespace FiscalCore.Servicos
{
    public interface ITransmitirSefazCommand
    {
        public Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope);

        async Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return await TransmitirAsync(url, envelope).ConfigureAwait(false);
        }
    }
}

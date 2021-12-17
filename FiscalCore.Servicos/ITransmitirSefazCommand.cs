using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.ValueObjects;
using System.Threading.Tasks;
using System.Xml;

namespace FiscalCore.Servicos
{
    public interface ITransmitirSefazCommand
    {
        public Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope);
        public Task<string> TransmitirAsync(distDFeInt distDFeInt, bool validarXmlConsulta = true);
    }
}

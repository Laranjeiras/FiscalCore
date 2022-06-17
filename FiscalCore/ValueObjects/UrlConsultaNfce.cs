using FiscalCore.Tipos;

namespace FiscalCore.ValueObjects
{
    public class UrlConsultaNfce
    {
        public UrlConsultaNfce(eTipoAmbiente tipoAmbiente, eUF uf, eVersaoQrCode versaoQrCode, string urlQrCode, string urlConsulta)
        {
            TipoAmbiente = tipoAmbiente;
            UF = uf;
            UrlQrCode = urlQrCode;
            UrlConsulta = urlConsulta;
            VersaoQrCode = versaoQrCode;
            UrlConsulta = urlConsulta;
        }

        public eTipoAmbiente TipoAmbiente { get; private set; }
        public eUF UF { get; private set; }
        public string UrlQrCode { get; private set; }
        public string UrlConsulta { get; private set; }
        public eVersaoQrCode VersaoQrCode { get; private set; }
    }
}

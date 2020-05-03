using FiscalCore.Main.Enums;
using System.Drawing;

namespace FiscalCore.Main.Configuracoes
{
    public interface IConfiguracaoDanfe
    {
        public eVersaoQrCode VersaoQrCode { get; set; }
        public string NFCeUrlConsultaSefaz { get; set; }
        public string NFCeUrlConsultaQrCodeSefaz { get; set; }
        public bool SegundaViaContingencia { get; set; }
        public FontFamily CarregarFontePadraoNfceNativa(string font = null);
    }
}

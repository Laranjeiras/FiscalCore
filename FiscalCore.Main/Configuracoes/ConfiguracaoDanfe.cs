using FiscalCore.Main.Enums;
using FiscalCore.Main.Properties;
using FiscalCore.Main.Utils;
using System.Drawing;
using System.Drawing.Text;

namespace FiscalCore.Main.Configuracoes
{
    public class ConfiguracaoDanfe : IConfiguracaoDanfe
    {
        public eVersaoQrCode VersaoQrCode { get; set; } = eVersaoQrCode.QrCodeVersao2;
        public string FontPadraoNfceNativa { get; set; }
        public string NFCeUrlConsultaSefaz { get; set; }
        public bool SegundaViaContingencia { get; set; }
        public string NFCeUrlConsultaQrCodeSefaz { get; set; }

        public FontFamily CarregarFontePadraoNfceNativa(string font = null)
        {
            if (font != null)
            {
                FontPadraoNfceNativa = font;
                return new FontFamily(font);
            }

            PrivateFontCollection colecaoDeFontes; //todo dispose na coleção
            var openSans = Fonte.CarregarDeByteArray(Resources.OpenSans_CondBold, out colecaoDeFontes);

            return openSans;
        }
    }
}
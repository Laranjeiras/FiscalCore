using FiscalCore.Properties;
using FiscalCore.Utils;
using FiscalCore.Tipos;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoDanfe : IConfiguracaoDanfe
    {
        public eVersaoQrCode VersaoQrCode { get; set; } = eVersaoQrCode.QrCodeVersao2;
        public string FontPadraoNfceNativa { get; set; }
        public string NFCeUrlConsultaSefaz { get; set; }
        public bool SegundaViaContingencia { get; set; }
        public string NFCeUrlConsultaQrCodeSefaz { get; set; }

        private string diretorioSalvarDanfe { get; set; }
        public string DiretorioSalvarDanfe
        {
            get => ObterDiretorioSalvarDanfe();
            set => DefinirDiretorioSalvarDanfe(value);  
        }

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

        private string ObterDiretorioSalvarDanfe()
        {
            if (diretorioSalvarDanfe == null || !Directory.Exists(diretorioSalvarDanfe))
                DefinirDiretorioSalvarDanfe(null);
            return diretorioSalvarDanfe;
        }

        private void DefinirDiretorioSalvarDanfe(string diretorioSalvarDanfe)
        {
            if (diretorioSalvarDanfe == null)
                diretorioSalvarDanfe = Path.Combine(Directory.GetCurrentDirectory(), "Danfes");

            Arquivo.CriarDiretorioSeNaoExistir(diretorioSalvarDanfe);
            this.diretorioSalvarDanfe = diretorioSalvarDanfe;
        }
    }
}
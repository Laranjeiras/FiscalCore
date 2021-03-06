using FiscalCore.Enums;
using FiscalCore.Properties;
using FiscalCore.Utils;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoDanfe : IConfiguracaoDanfe
    {
        public VersaoQrCode VersaoQrCode { get; set; } = VersaoQrCode.QrCodeVersao2;
        public string FontPadraoNfceNativa { get; set; }
        public string NFCeUrlConsultaSefaz { get; set; }
        public bool SegundaViaContingencia { get; set; }
        public string NFCeUrlConsultaQrCodeSefaz { get; set; }

        private string _diretorioSalvarDanfe { get; set; }
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
            if (_diretorioSalvarDanfe == null || !Directory.Exists(_diretorioSalvarDanfe))
                return Arquivo.CriarDiretorioNaRaizDoApp("Danfes");
            return _diretorioSalvarDanfe;
        }

        private void DefinirDiretorioSalvarDanfe(string diretorioSalvarDanfe)
        {
            if (diretorioSalvarDanfe == null)
                diretorioSalvarDanfe = Arquivo.CriarDiretorioNaRaizDoApp("Danfes");
            Arquivo.CriarDiretorioSeNaoExistir(diretorioSalvarDanfe);
            _diretorioSalvarDanfe = diretorioSalvarDanfe;
        }
    }
}
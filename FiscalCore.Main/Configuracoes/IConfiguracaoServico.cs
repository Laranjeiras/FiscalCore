using FiscalCore.Main.Enums;
using FiscalCore.Main.Models.Emitente;

namespace FiscalCore.Main.Configuracoes
{
    public interface IConfiguracaoServico
    {
        public int NFeSerie { get; set; }
        public int NFCeSerie { get; set; }
        public eTipoAmbiente TipoAmbiente { get; set; }
        public emit Emitente { get; set; }
        public ConfiguracaoCertificado Certificado { get; set; }
        public IConfiguracaoDanfe ConfigDanfe { get; set; }
        public string DiretorioSalvarXml { get; set; }
        public string DiretorioSalvarDanfe { get; set; }
        public string DiretorioSchemas { get; set; }
        public eUF UF { get; set; }
        public ConfiguracaoCsc Csc { get; set; }
        public eVersaoServico VersaoAutorizacaoNFe { get; set; }
        public string ImpressoraCupom { get; set; }
    }
}

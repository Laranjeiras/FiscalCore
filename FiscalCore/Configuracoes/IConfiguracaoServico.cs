using FiscalCore.Enums;
using FiscalCore.Modelos.Emitente;

namespace FiscalCore.Configuracoes
{
    public interface IConfiguracaoServico
    {
        public int NFeSerie { get; set; }
        public int NFCeSerie { get; set; }
        public eTipoAmbiente TipoAmbiente { get; set; }
        public emit Emitente { get; set; }
        public ConfiguracaoCertificado ConfigCertificado { get; set; }
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

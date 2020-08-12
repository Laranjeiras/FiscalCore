using FiscalCore.Enums;
using FiscalCore.Modelos.Emitente;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoServico : IConfiguracaoServico
    {
        public ConfiguracaoServico(
            eTipoAmbiente tipoAmbiente,
            eUF uf,
            ConfiguracaoCertificado certificado,
            emit emitente,
            ConfiguracaoCsc csc,
            IConfiguracaoDanfe configDanfe
        )
        {
            this.Certificado = certificado;
            this.TipoAmbiente = tipoAmbiente;
            this.UF = uf;
            this.Emitente = emitente;
            this.Csc = csc;

            TipoEmissao = eTipoEmissao.Normal;

            VersaoCancelamentoNFe = eVersaoServico.Versao100;
            VersaoInutilizacaoNFe = eVersaoServico.Versao400;
            VersaoAutorizacaoNFe = eVersaoServico.Versao400;
            ConfigDanfe = configDanfe;
        }

        public ConfiguracaoCertificado Certificado { get; set; }
       
        public eTipoAmbiente TipoAmbiente { get; set; }

        public eTipoEmissao TipoEmissao { get; set; }

        public eUF UF { get; set; }

        public string DiretorioSalvarXml { get; set; }
        public string DiretorioSchemas { get; set; }
        public string DiretorioSalvarDanfe { get; set; }

        public emit Emitente { get; set; }

        public IConfiguracaoDanfe ConfigDanfe { get; set; }
        public ConfiguracaoCsc Csc { get; set; }
        public int TimeOut { get; set; } = 5000;
        public string VersaoProc { get; set; }

        public eVersaoServico VersaoInutilizacaoNFe { get; set; }
        public eVersaoServico VersaoCancelamentoNFe { get; set; }
        public eVersaoServico VersaoAutorizacaoNFe { get; set; }
        public string ImpressoraCupom { get; set; }
        public int NFeSerie { get; set; }
        public int NFCeSerie { get; set; }
    }
}

using FiscalCore.Main.Enums;
using FiscalCore.Main.Models.Emitente;

namespace FiscalCore.Main.Configuracoes
{
    public class ConfiguracaoServico
    {
        public ConfiguracaoServico(eTipoAmbiente tipoAmbiente, eUF uf, ConfiguracaoCertificado certificado, emit emitente, ConfiguracaoCsc csc)
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
        }

        public ConfiguracaoCertificado Certificado { get; set; }
       
        public eTipoAmbiente TipoAmbiente { get; private set; }

        public eTipoEmissao TipoEmissao { get; set; }

        public eUF UF { get; private set; }

        public string DiretorioSalvarXml { get; set; }

        public string DiretorioSchemas { get; set; }

        public emit Emitente { get; private set; }

        public ConfiguracaoCsc Csc { get; private set; }
        public eTipoImpressao NFeTpImp { get; protected set; }
        public eTipoImpressao NFCeTpImp { get; protected set; }
        public int TimeOut { get; set; } = 5000;
        public string VersaoProc { get; set; }

        public eVersaoServico VersaoInutilizacaoNFe { get; set; }
        public eVersaoServico VersaoCancelamentoNFe { get; set; }
        public eVersaoServico VersaoAutorizacaoNFe { get; set; }
    }
}

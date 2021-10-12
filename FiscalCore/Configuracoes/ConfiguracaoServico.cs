using FiscalCore.Exceptions;
using FiscalCore.Modelos.NotaFiscal.Emitente;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoServico
    {
        public ConfiguracaoServico()
        {

        }

        public ConfiguracaoServico(
            eTipoAmbiente tipoAmbiente,
            eUF uf,
            ConfiguracaoCertificado configCertificado,
            emit emitente,
            ConfiguracaoCsc csc,
            IConfiguracaoDanfe configDanfe
        )
        {
            this.ConfigCertificado = configCertificado;
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

        public ConfiguracaoCertificado ConfigCertificado { get; set; }
       
        public eTipoAmbiente TipoAmbiente { get; set; }

        public eTipoEmissao TipoEmissao { get; set; }

        public eUF UF { get; set; } = eUF.RJ;

        private string _diretorioSalvarXml;
        public string DiretorioSalvarXml
        {
            get { return _diretorioSalvarXml ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSalvarXml = value; }
        }

        private string _diretorioSchemas;
        public string DiretorioSchemas
        {
            get { return _diretorioSchemas ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSchemas = value; }
        }

        public string _diretorioSalvarDanfe;
        public string DiretorioSalvarDanfe
        {
            get { return _diretorioSalvarDanfe ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSalvarDanfe = value; }
        }

        public emit Emitente { get; set; }

        public IConfiguracaoDanfe ConfigDanfe { get; set; }
        public ConfiguracaoCsc Csc { get; set; }
        public int TimeOut { get; set; } = 5000;


        /// <summary>
        /// Versão do Processo de emissão da NF-e.
        /// </summary>
        public string VersaoProc { get; set; }

        public eVersaoServico VersaoInutilizacaoNFe { get; set; }
        public eVersaoServico VersaoCancelamentoNFe { get; set; }
        public eVersaoServico VersaoAutorizacaoNFe { get; set; }
        public string ImpressoraCupom { get; set; }
        public int NFeSerie { get; set; }
        public int NFCeSerie { get; set; }

        public bool Valido => valido();

        private bool valido()
        {
            try
            {
                Validar();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Validar()
        {
            if (Emitente == null)
                throw new FalhaValidacaoException("Emitente não informado");
            if (Emitente.enderEmit == null)
                throw new FalhaValidacaoException("Endereço do emitente não informado");
            if (Emitente.enderEmit.cMun <= 0)
                throw new FalhaValidacaoException("Municipio do emitente inválido");
            if ((int)VersaoAutorizacaoNFe == 0)
                throw new FalhaValidacaoException("Versão autorização NFe não informado");
            if ((int)VersaoCancelamentoNFe == 0)
                throw new FalhaValidacaoException("Versão cancelamento NFe não informado");
            if ((int)VersaoInutilizacaoNFe == 0)
                throw new FalhaValidacaoException("Versão inutilização NFe não informado");
        }
    }
}

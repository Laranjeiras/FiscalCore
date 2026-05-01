using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoBasicaServico
    {
        public ConfiguracaoCertificado ConfigCertificado { get; set; } = null!;
        public string CNPJEmitente { get; set; } = null!;
        public eTipoAmbiente TipoAmbiente { get; set; } = eTipoAmbiente.Homologacao;
        public eUF UF { get; set; } = eUF.RJ;
        public int TimeOut { get; set; } = 5000;

        /// <summary>
        /// Ignora erro caso de erro ao salvar arquivos xml no storage
        /// </summary>
        public bool IgnorarErroDeStorage { get; set; }

        private string? _diretorioSchemas;
        public string DiretorioSchemas
        {
            get { return _diretorioSchemas ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSchemas = value; }
        }

        /// <summary>
        /// Validar Xml com Schema
        /// </summary>
        public bool ValidarXmlSchema { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(CNPJEmitente) || CNPJEmitente.Length < 11 && CNPJEmitente.Length > 18)
            {
                throw new ConfiguracaoException($"{nameof(CNPJEmitente)} inválido");
            }
        }
    }
}

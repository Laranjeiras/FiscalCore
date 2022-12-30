using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoBasicaServico
    {
        public ConfiguracaoCertificado ConfigCertificado { get; set; }
        public string CNPJEmitente { get; set; }
        public eTipoAmbiente TipoAmbiente { get; set; } = eTipoAmbiente.Homologacao;
        public eUF UF { get; set; } = eUF.RJ;
        public int TimeOut { get; set; } = 5000;

        private string _diretorioSchemas;
        public string DiretorioSchemas
        {
            get { return _diretorioSchemas ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSchemas = value; }
        }

        public void Validar()
        {
            if (string.IsNullOrEmpty(CNPJEmitente) || CNPJEmitente.Length < 11 && CNPJEmitente.Length > 18)
            {
                throw new ConfiguracaoExcepetion($"{nameof(CNPJEmitente)} inválido");
            }
        }
    }
}

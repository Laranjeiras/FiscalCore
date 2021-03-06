using FiscalCore.Enums;

namespace FiscalCore.Servicos.Utils
{
    public class SefazUrl
    {
        public string Url { get; private set; }
        public string Action { get; private set; }
        public eModeloDocumento ModeloDocumento { get; private set; }
        public eTipoAmbiente TipoAmbiente { get; private set; }
        public eUF UF { get; private set; }
        public TipoServico Servico { get; set; }

        public SefazUrl(TipoServico servico, eUF uf, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, string url, string action)
        {
            Url = url;
            Action = action;
            UF = uf;
            TipoAmbiente = tipoAmbiente;
            ModeloDocumento = modeloDocumento;
            Servico = servico;
        }
    }
}

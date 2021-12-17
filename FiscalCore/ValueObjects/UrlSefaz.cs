using FiscalCore.Tipos;

namespace FiscalCore.ValueObjects
{
    public class UrlSefaz
    {
        public string Url { get; private set; }
        public string Action { get; private set; }
        public eModeloDocumento ModeloDocumento { get; private set; }
        public eTipoAmbiente TipoAmbiente { get; private set; }
        public eUF UF { get; private set; }
        public eTipoServico Servico { get; private set; }

        public UrlSefaz(eTipoServico servico, eUF uf, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, string url, string action)
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

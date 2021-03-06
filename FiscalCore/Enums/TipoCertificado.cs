using System.ComponentModel;

namespace FiscalCore.Enums
{
    public enum TipoCertificado
    {
        [Description("Certificado A1 Repositório")]
        A1Repositorio = 0,
        [Description("Certificado A1 em arquivo")]
        A1Arquivo = 1,
        [Description("Certificado A3")]
        A3 = 2
    }
}

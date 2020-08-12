using System.ComponentModel;

namespace FiscalCore.Enums
{
    public enum eTipoCertificado
    {
        [Description("Certificado A1")]
        A1Repositorio = 0,
        [Description("Certificado A1 em arquivo")]
        A1Arquivo = 1,
        [Description("Certificado A3")]
        A3 = 2,
        [Description("Certificado A1 em byte array")]
        A1ByteArray = 3
    }
}

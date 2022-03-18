using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eIdentificadorOperacaoCsc
    {
        [XmlEnum("1")]
        ConsultaCscAtivos = 1,
        [XmlEnum("2")]
        SolicitaNovoCsc = 2,
        [XmlEnum("3")]
        RevogaCscAtivo = 3
    }
}
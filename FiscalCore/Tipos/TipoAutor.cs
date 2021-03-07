using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoAutor
    {
        [XmlEnum("1")]
        [Description("Empresa Emitente")]
        EmpresaEmitente = 1,
        [XmlEnum("2")]
        [Description("Empresa Destinatária")]
        EmpresaDestinataria = 2,
        [XmlEnum("3")]
        [Description("Empresa")]
        Empresa = 3,
        [XmlEnum("5")]
        [Description("Fisco")]
        Fisco = 5,
        [XmlEnum("6")]
        [Description("RFB")]
        RFB = 6,
        [XmlEnum("9")]
        [Description("Outros Órgãos")]
        OutrosOrgaos = 9
    }
}

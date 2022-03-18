using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum ePresencaComprador
    {
        [XmlEnum("0")]
        [Description("Não se aplica")]
        NaoSeAplica = 0,
        [XmlEnum("1")]
        [Description("Operação presencial")]
        Presencial = 1,
        [XmlEnum("2")]
        [Description("Operação não presencial, pela Internet")]
        Internet = 2,
        [XmlEnum("3")]
        [Description("Operação não presencial, Teleatendimento")]
        Teleatendimento = 3,
        [XmlEnum("4")]
        [Description("NFC-e em operação com entrega a domicílio")]
        EntregaDomicilio = 4,
        [XmlEnum("5")]
        [Description("Fora do estabelecimento")]
        ForaEstabelecimento = 5,
        [XmlEnum("9")]
        [Description("Operação não presencial, outros.")]
        NaoPresencialOutros = 9
    }
}

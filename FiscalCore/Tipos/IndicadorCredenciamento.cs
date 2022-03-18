using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eIndicadorCredenciamentoNfe
    {
        [XmlEnum("0")] NaoCredenciado = 0,
        [XmlEnum("1")] Credenciado = 1,
        [XmlEnum("2")] CredenciadoTodasOperacoes = 2,
        [XmlEnum("3")] CredenciadoParcial = 3,
        [XmlEnum("4")] SemInformacaoSefaz = 4
    }
}

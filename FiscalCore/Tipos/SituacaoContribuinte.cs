using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum SituacaoContribuinte
    {
        [XmlEnum("0")] 
        NaoHabilitado = 0,
        [XmlEnum("1")] 
        Habilitado = 1
    }
}

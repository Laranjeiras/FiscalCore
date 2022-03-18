using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum TipoTransporteInternacional
    {
        /// <summary>
        ///     1=Marítima
        /// </summary>
        [XmlEnum("1")] Maritima = 1,

        /// <summary>
        ///     2=Fluvial
        /// </summary>
        [XmlEnum("2")] Fluvial = 2,

        /// <summary>
        ///     3=Lacustre
        /// </summary>
        [XmlEnum("3")] Lacustre = 3,

        /// <summary>
        ///     4=Aérea
        /// </summary>
        [XmlEnum("4")] Aerea = 4,

        /// <summary>
        ///     5=Postal
        /// </summary>
        [XmlEnum("5")] Postal = 5,

        /// <summary>
        ///     6=Ferroviária
        /// </summary>
        [XmlEnum("6")] Ferroviaria = 6,

        /// <summary>
        ///     7=Rodoviária
        /// </summary>
        [XmlEnum("7")] Rodoviaria = 7,

        /// <summary>
        ///     8=Conduto / Rede de Transmissão
        /// </summary>
        [XmlEnum("8")] CondutoRedeTransmissão = 8,

        /// <summary>
        ///     9=Meios Próprios
        /// </summary>
        [XmlEnum("9")] MeiosProprios = 9,

        /// <summary>
        ///     10=Entrada / Saída ficta
        /// </summary>
        [XmlEnum("10")] EntradaSaidaficta = 10,

        /// <summary>
        ///     11=Courier
        /// </summary>
        [XmlEnum("11")] Courier = 11,

        /// <summary>
        ///     12=Handcarry (NT 2013/005 v 1.10)
        /// </summary>
        [XmlEnum("12")] Handcarry = 12
    }
}

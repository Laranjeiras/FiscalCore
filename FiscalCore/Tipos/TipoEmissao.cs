using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{

    /// <summary>
    ///     Forma de emissão da NF-e
    ///     <para>1 - Emissão normal (não em contingência)</para>
    ///     <para>2 - Contingência FS-IA, com impressão do DANFE em formulário de segurança</para>
    ///     <para>3 - Contingência SCAN (Sistema de Contingência do Ambiente Nacional)</para>
    ///     <para>4 - Contingência DPEC (Declaração Prévia da Emissão em Contingência)</para>
    ///     <para>5 - Contingência FS-DA, com impressão do DANFE em formulário de segurança</para>
    ///     <para>6 - Contingência SVC-AN (SEFAZ Virtual de Contingência do AN)</para>
    ///     <para>7 - Contingência SVC-RS (SEFAZ Virtual de Contingência do RS)</para>
    ///     <para>9 - Contingência off-line da NFC-e</para>
    ///     <para>Nota: Para a NFC-e somente estão disponíveis e são válidas as opções de contingência 5 e 9</para>
    /// </summary>
    public enum eTipoEmissao
    {
        [XmlEnum("1")]
        [Description("Normal")]
        Normal = 1,

        [XmlEnum("2")]
        [Description("Contingência FS-IA")]
        FSIA = 2,

        [XmlEnum("3")]
        [Description("Contingência SCAN")]
        SCAN = 3,

        [XmlEnum("4")]
        [Description("Contingência DPEC")]
        EPEC = 4,

        [XmlEnum("5")]
        [Description("Contingência FS-DA")]
        FSDA = 5,

        [XmlEnum("6")]
        [Description("Contingência SVC-AN")]
        SVCAN = 6,

        [XmlEnum("7")]
        [Description("Contingência SVC-RS")]
        SVCRS = 7,

        [XmlEnum("9")]
        [Description("Contingência off-line")]
        OffLine = 9
    }

}

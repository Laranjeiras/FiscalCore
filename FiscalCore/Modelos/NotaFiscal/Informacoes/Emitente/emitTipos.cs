


// Autores: 







#region

using System.Xml.Serialization;

#endregion

namespace FiscalCore.NotaFiscal.Informacoes.Emitente
{
    /// <summary>
    ///     <para>1 – Simples Nacional;</para>
    ///     <para>2 – Simples Nacional – excesso de sublimite de receita bruta;</para>
    ///     <para>3 – Regime Normal. (v2.0).</para>
    /// </summary>
    public enum CRT
    {
        /// <summary>
        ///     1 – Simples Nacional
        /// </summary>
        [XmlEnum("1")] SimplesNacional = 1,

        /// <summary>
        ///     2 – Simples Nacional – excesso de sublimite de receita bruta
        /// </summary>
        [XmlEnum("2")] SimplesNacionalExcessoSublimite = 2,

        /// <summary>
        ///     3 – Regime Normal
        /// </summary>
        [XmlEnum("3")] RegimeNormal = 3
    }
}
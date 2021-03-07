using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.DistribuicaoDFe.Modelos
{
    /// <summary>
    ///     A09 - Grupo para consultar um DF-e a partir de um NSU específico
    /// </summary>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class consNSU
    {
        public string NSU { get; set; }
    }
}

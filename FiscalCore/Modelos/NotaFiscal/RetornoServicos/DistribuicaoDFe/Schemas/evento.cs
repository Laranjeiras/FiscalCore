


// Autores: 







#region

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace FiscalCore.NotaFiscal.RetornoServicos.DistribuicaoDFe.Schemas
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class evento
    {
        #region Propriedades

        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe", ElementName = "infEvento")]
        public eventoInfEvento infEvento { get; set; }

        [XmlAttribute] public decimal versao { get; set; }

        #endregion
    }
}
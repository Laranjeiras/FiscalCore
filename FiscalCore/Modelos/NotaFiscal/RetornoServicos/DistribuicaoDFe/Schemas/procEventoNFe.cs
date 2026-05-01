#nullable disable
#pragma warning disable CS8981



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
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class procEventoNFe
    {
        #region Propriedades

        [XmlAttribute] public decimal versao { get; set; }

        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public evento evento { get; set; }

        public retEvento retEvento { get; set; }

        #endregion
    }
}
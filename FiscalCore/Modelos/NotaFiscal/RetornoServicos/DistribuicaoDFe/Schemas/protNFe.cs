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
    public class protNFe
    {
        #region Propriedades

        public infProt infProt { get; set; }

        [XmlAttribute] public decimal versao { get; set; }

        #endregion
    }
}
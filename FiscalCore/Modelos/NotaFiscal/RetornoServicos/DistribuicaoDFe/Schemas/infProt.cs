


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
    public class infProt
    {
        #region Propriedades

        public byte tpAmb { get; set; }

        public decimal verAplic { get; set; }

        [XmlElement(DataType = "integer")] public string chNFe { get; set; }

        public DateTime dhRecbto { get; set; }

        public ulong nProt { get; set; }

        public string digVal { get; set; }

        public byte cStat { get; set; }

        public string xMotivo { get; set; }

        [XmlAttribute] public string Id { get; set; }

        #endregion
    }
}
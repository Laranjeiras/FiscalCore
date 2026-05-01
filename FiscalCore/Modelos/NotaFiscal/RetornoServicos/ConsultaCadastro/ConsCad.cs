#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

using System.Xml.Serialization;

#endregion

namespace FiscalCore.NotaFiscal.RetornoServicos.ConsultaCadastro
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class ConsCad
    {
        #region Propriedades

        /// <summary>
        ///     GP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     GP03 - Dados da consulta
        /// </summary>
        public infConsEnv infCons { get; set; }

        #endregion
    }
}
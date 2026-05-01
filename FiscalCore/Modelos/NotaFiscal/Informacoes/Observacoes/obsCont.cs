#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

using System.Xml.Serialization;

#endregion

namespace FiscalCore.NotaFiscal.Informacoes.Observacoes
{
    public class obsCont
    {
        #region Propriedades

        /// <summary>
        ///     Z05 - Identificação do campo
        /// </summary>
        [XmlAttribute]
        public string xCampo { get; set; }

        /// <summary>
        ///     Z06 - Conteúdo do campo
        /// </summary>
        public string xTexto { get; set; }

        #endregion
    }
}
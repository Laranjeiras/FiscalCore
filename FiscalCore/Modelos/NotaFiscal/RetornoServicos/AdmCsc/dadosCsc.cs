


// Autores: 







#region

using System;

#endregion

namespace FiscalCore.NotaFiscal.RetornoServicos.AdmCsc
{
    [Serializable]
    public class dadosCsc
    {
        #region Propriedades

        /// <summary>
        ///     AP07 / AR08 - Número identificador do CSC
        /// </summary>
        public string idCsc { get; set; }

        /// <summary>
        ///     AP08 / AR09 - Código alfanumérico do CSC
        /// </summary>
        public string codigoCsc { get; set; }

        #endregion
    }
}
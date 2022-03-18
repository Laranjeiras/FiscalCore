


// Autores: 







namespace FiscalCore.NotaFiscal.RetornoServicos.Download
{
    public class procNFeGrupoZip
    {
        #region Propriedades

        /// <summary>
        ///     JR18 - XML da NF-e compactado no padrão gZip, o tipo do campo é base64Binary.
        /// </summary>
        public byte[] NFeZip { get; set; }

        /// <summary>
        ///     JR19 - Protocolo de Autorização de Uso compactado no padrão gZip, o tipo do campo é base64Binary
        /// </summary>
        public byte[] protNFeZip { get; set; }

        #endregion
    }
}
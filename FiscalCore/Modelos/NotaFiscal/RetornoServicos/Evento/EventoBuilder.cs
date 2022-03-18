
namespace FiscalCore.NotaFiscal.RetornoServicos.Evento
{
    public class EventoBuilder
    {
        #region Propriedades

        /// <summary>
        /// Sequencial do evento
        /// </summary>
        public int SeqEvento { get; private set; }
        /// <summary>
        ///     Protocolo de Autorização
        /// </summary>
        public string ProtAutorizacao { get; private set; }

        /// <summary>
        ///     Chave da NFe
        /// </summary>
        public string ChaveNFe { get; private set; }

        /// <summary>
        ///     Justificativa
        /// </summary>
        public string Justificativa { get; private set; }

        /// <summary>
        ///     Documento CPF ou CNPJ
        /// </summary>
        public string Cpfcnpj { get; private set; }

        #endregion

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public EventoBuilder(int seqEvento, string protAutorizacao,
            string chaveNFe, string justificativa, string cpfcnpj)
        {
            SeqEvento = seqEvento;
            ProtAutorizacao = protAutorizacao;
            ChaveNFe = chaveNFe;
            Justificativa = justificativa;
            Cpfcnpj = cpfcnpj;  
        }
    }
}
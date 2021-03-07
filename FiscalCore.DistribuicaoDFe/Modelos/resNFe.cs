using System;

namespace FiscalCore.DistribuicaoDFe.Modelos
{
    public class resNFe
    {
        public string chNFe { get; set; }

        /// <summary>
        /// C04 - CNPJ do Emitente
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        /// C05 - CPF do Emitente
        /// </summary>
        public string CPF { get; set; }

        /// <summary>
        /// C06 - Razão Social ou Nome do Emitente
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// C07 - IE do Emitente. Valores válidos: 
        /// vazio  (não contribuinte do ICMS), 
        /// ISENTO (contribuinte do ICMS ISENTO de Inscrição no Cadastro de Contribuintes) ou 
        /// IE (Contribuinte do ICMS)
        /// </summary>
        public string IE { get; set; }

        /// <summary>
        /// C10 - Valor Total da NF-e
        /// </summary>
        public decimal vNF { get; set; }

        /// <summary>
        /// C11 - Digest Value da NF-e na base de dados do Ambiente Nacional
        /// </summary>
        public string digVal { get; set; }

        /// <summary>
        /// C12 - Data de autorização da NF-e
        /// </summary>
        public DateTime dhRecbto { get; set; }

        /// <summary>
        /// C13 - Número de protocolo da NF-e
        /// </summary>
        public ulong nProt { get; set; }

        /// <summary>
        /// C14 - Situação da NF-e: 1=Uso autorizado; 2=Uso denegado.
        /// </summary>
        public byte cSitNFe { get; set; }
    }
}

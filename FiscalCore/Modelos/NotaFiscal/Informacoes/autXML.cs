#nullable disable
#pragma warning disable CS8981
using System;
using System.Collections.Generic;

namespace FiscalCore.NotaFiscal.Informacoes
{
    public class autXML
    {
        #region Propriedades

        /// <summary>
        ///     GA02 - CNPJ Autorizado
        /// </summary>
        public string CNPJ
        {
            get { return cnpj; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (string.IsNullOrEmpty(cpf))
                    cnpj = value;
                else
                {
                    throw new ArgumentException(ErroCpfCnpjPreenchidos);
                }
            }
        }

        /// <summary>
        ///     GA03 - CPF Autorizado
        /// </summary>
        public string CPF
        {
            get { return cpf; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (string.IsNullOrEmpty(cnpj))
                    cpf = value;
                else
                {
                    throw new ArgumentException(ErroCpfCnpjPreenchidos);
                }
            }
        }

        private const string ErroCpfCnpjPreenchidos = "Somente preencher um dos campos: CNPJ ou CPF, para um objeto do tipo autXML!";

        #endregion

        #region Variaveis Globais

        private string cnpj;
        private string cpf;

        public override global::System.Boolean Equals(global::System.Object obj)
        {
            return obj is autXML xML &&
                   CNPJ == xML.CNPJ &&
                   CPF == xML.CPF &&
                   cnpj == xML.cnpj &&
                   cpf == xML.cpf;
        }

        public override global::System.Int32 GetHashCode()
        {
            global::System.Int32 hashCode = 1811742084;
            hashCode = hashCode * -1521134295 + EqualityComparer<global::System.String>.Default.GetHashCode(CNPJ);
            hashCode = hashCode * -1521134295 + EqualityComparer<global::System.String>.Default.GetHashCode(CPF);
            hashCode = hashCode * -1521134295 + EqualityComparer<global::System.String>.Default.GetHashCode(cnpj);
            hashCode = hashCode * -1521134295 + EqualityComparer<global::System.String>.Default.GetHashCode(cpf);
            return hashCode;
        }

        #endregion
    }
}

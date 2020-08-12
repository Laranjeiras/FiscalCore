using System;
using System.Linq;
using Zion.Common2.Assertions;

namespace FiscalCore.Validacoes.Sefaz
{
    public class DFeAssertion
    {
        public static void uCom_IsValid(string value, string message = "I09 - Unidade Comercial - Unidade Comercial inválida, deve conter entre 1 e 6 caracteres")
        {
            ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value, message);
            ZionAssertion.StringHasMaxLen(value, 6, message);
        }

        public static void cNF_IsValid(string value, string message = "B03 - Código Numérico que compõe a Chave de Acesso inválido")
        {
            var invalid_cNF = new string[] { "00000000", "11111111", "22222222", "33333333", "44444444", "55555555", "66666666", "77777777", "88888888", "99999999", "12345678", "23456789", "34567890", "45678901", "56789012", "67890123", "78901234", "89012345", "90123456", "01234567" };

            if (invalid_cNF.Contains(value))
                throw new Exception("B03-10: cNF Código numérico em formato inválido");

            ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value, message);
            ZionAssertion.StringHasLen(value, 8, message);
        }

        public static bool cNF_IsValid(string value)
        {
            try 
            {
                cNF_IsValid(value, "B03 - Código Numérico que compõe a Chave de Acesso inválido");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

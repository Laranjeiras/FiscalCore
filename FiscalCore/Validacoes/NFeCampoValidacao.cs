using FiscalCore.Utils;
using System;
using System.Linq;

namespace FiscalCore.Validacoes
{
    [Obsolete]
    public static class NFeCampoValidacao
    {
        public static Notificacao uCom_Validar(string value, string mensagem = "I09 - Unidade Comercial - Unidade Comercial inválida, deve conter entre 1 e 6 caracteres")
        {
            var valido = !string.IsNullOrEmpty(value) &&
                            !(value.Length >= 1 && value.Length <= 6);
            if (!valido) 
                return new Notificacao("uCom", mensagem);

            return new Notificacao("uCom", "Válido", false);
        }

        public static Notificacao cNF_Validar(string value, string mensagem = "B03-10: Código numérico em formato inválido")
        {
            var invalid_cNF = new string[] { "00000000", "11111111", "22222222", "33333333", "44444444", "55555555", "66666666", "77777777", "88888888", "99999999", "12345678", "23456789", "34567890", "45678901", "56789012", "67890123", "78901234", "89012345", "90123456", "01234567" };

            if (invalid_cNF.Contains(value))
                return new Notificacao("cNF", mensagem);
            if (string.IsNullOrEmpty(value)) 
                return new Notificacao("cNF", mensagem);
            if (!(value.Length == 8))
                return new Notificacao("cNF", mensagem);

            return new Notificacao("cNF", "Válido", false);
        }
    }
}

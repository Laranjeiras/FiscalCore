using FiscalCore.Modelos.NotaFiscal.Endereco;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System.Collections.Generic;

namespace FiscalCore.Validacoes.Sefaz
{
    public class DFeAddressValidation
    {
        public Contrato Contrato { get; private set; } = new Contrato();

        BaseEndereco _address;

        public DFeAddressValidation(BaseEndereco address)
        {
            _address = address;
        }

        public DFeAddressValidation(string xLgr, string nro, string xCpl, string xBairro, long cMun, string xMun, eUF UF, string cep, int? cPais, string xPais, long? fone) 
        {
            this._address = new BaseEndereco(xLgr, nro, xCpl, xBairro, cMun, xMun, UF, cep, cPais, xPais, fone);
        }

        public bool IsValid()
        {
            if (!AddressValidation.xLgr_IsValid(_address.xLgr))
                Contrato.Add(new Notificacao("xLgr", "Logradouro inválido"));
            if (!AddressValidation.nro_IsValid(_address.nro))
                Contrato.Add(new Notificacao("nro", "Número do endereço inválido"));
            if (!AddressValidation.xBairro_IsValid(_address.xBairro))
                Contrato.Add(new Notificacao("xBairro", "Bairro inválido"));
            if (!AddressValidation.cMun_IsValid(_address.cMun))
                Contrato.Add(new Notificacao("cMun", "Município inválido"));
            if (!AddressValidation.xMun_IsValid(_address.xMun))
                Contrato.Add(new Notificacao("xMun", "Município inválido"));
            if (!AddressValidation.UF_IsValid(_address.UF.ToString()))
                Contrato.Add(new Notificacao("UF", "UF inválido"));

            return Contrato.Valido;
        }
    }

    /// <summary>
    /// Alterar a Exception pra validação com boolean
    /// </summary>
    internal class AddressValidation 
    {
        internal static bool xLgr_IsValid(string value)
        {
            try
            {
                //ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                //ZionAssertion.StringHasMinLen(value, 2);
                //ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool nro_IsValid(string value)
        {
            try
            {
                //ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                //ZionAssertion.StringHasMinLen(value, 1);
                //ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool xCpl_IsValid(string value)
        {
            try
            {
                if (value == null) return true;
                //ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool xBairro_IsValid(string value)
        {
            try
            {
                //ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                //ZionAssertion.StringHasMinLen(value, 2);
                //ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool cMun_IsValid(long value)
        {
            if(value < 1000000 || value > 9999999)
                return false;
            return true;
        }

        public static bool xMun_IsValid(string value)
        {
            try
            {
                //ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                //ZionAssertion.StringHasMinLen(value, 2);
                //ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UF_IsValid(string value)
        {
            try
            {
                //ZionAssertion.StringHasMinLen(value, 2);
                //ZionAssertion.StringHasMaxLen(value, 2);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

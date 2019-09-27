using FiscalCore.Main.Enums;
using FiscalCore.Main.Extensions;
using FiscalCore.Main.Models.Endereco;
using System.Collections.Generic;
using Zion.Common.Assertions;
using Zion.Common.Flunt.Notifications;

namespace FiscalCore.Main.ValidationsSefaz
{
    public class DFeAddressValidation
    {
        public IList<Notification> Notifications = new List<Notification>();

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
                Notifications.Add(new Notification("xLgr", "Logradouro inválido"));
            if (!AddressValidation.nro_IsValid(_address.nro))
                Notifications.Add(new Notification("nro", "Número do endereço inválido"));
            if (!AddressValidation.xBairro_IsValid(_address.xBairro))
                Notifications.Add(new Notification("xBairro", "Bairro inválido"));
            if (!AddressValidation.cMun_IsValid(_address.cMun))
                Notifications.Add(new Notification("cMun", "Município inválido"));
            if (!AddressValidation.xMun_IsValid(_address.xMun))
                Notifications.Add(new Notification("xMun", "Município inválido"));
            if (!AddressValidation.UF_IsValid(_address.UF.ToString()))
                Notifications.Add(new Notification("UF", "UF inválido"));

            return Notifications.Count == 0;
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
                ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                ZionAssertion.StringHasMinLen(value, 2);
                ZionAssertion.StringHasMaxLen(value, 60);
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
                ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                ZionAssertion.StringHasMinLen(value, 1);
                ZionAssertion.StringHasMaxLen(value, 60);
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
                ZionAssertion.StringHasMaxLen(value, 60);
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
                ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                ZionAssertion.StringHasMinLen(value, 2);
                ZionAssertion.StringHasMaxLen(value, 60);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool cMun_IsValid(long value)
        {
            if (!Zion.Common.Helpers.NumericHelper.IsBetweenOrEqual(value, 1000000, 9999999))
                return false;
            return true;
        }

        public static bool xMun_IsValid(string value)
        {
            try
            {
                ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(value);
                ZionAssertion.StringHasMinLen(value, 2);
                ZionAssertion.StringHasMaxLen(value, 60);
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
                ZionAssertion.StringHasMinLen(value, 2);
                ZionAssertion.StringHasMaxLen(value, 2);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

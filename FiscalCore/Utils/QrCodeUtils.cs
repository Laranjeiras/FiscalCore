using FiscalCore.Fabrica;
using FiscalCore.NotaFiscal;
using FiscalCore.Tipos;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FiscalCore.Utils
{
    public static class QrCodeUtils
    {
        public static string ObterUrlQrCode2ComParametro(eTipoAmbiente tipoAmbiente, eUF uf)
        {
            const string interrogacao = "?";
            const string parametro = "p=";

            var url = FabricarUrl.ObterUrlConsultaNfce(tipoAmbiente, uf, eVersaoQrCode.QrCodeVersao2);

            var urlQrCode = url.UrlQrCode;

            if (!urlQrCode.EndsWith(interrogacao))
                urlQrCode += interrogacao;
            if (!urlQrCode.EndsWith(parametro))
                urlQrCode += parametro;
            return urlQrCode;
        }

        public static string ObterHexSha1DeString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return ObterHexDeByteArray(hashBytes);
        }

        public static string ObterHexDeByteArray(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string ObterHexDeString(string s)
        {
            var hex = "";
            foreach (var c in s)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        private static string ObterUrlQrCode2(NFe nfe, string cIdToken, string csc)
        {
            var url = QrCodeUtils.ObterUrlQrCode2ComParametro(nfe.infNFe.ide.tpAmb, nfe.infNFe.ide.cUF);

            const string pipe = "|";

            var chave = nfe.infNFe.Id.Substring(3);

            var ambiente = (int)nfe.infNFe.ide.tpAmb;

            var idCsc = Convert.ToInt32(cIdToken);

            string dadosBase;

            if (nfe.infNFe.ide.tpEmis == eTipoEmissao.OffLine)
            {
                var diaEmi = nfe.infNFe.ide.dhEmi.Day.ToString("D2");
                var valorNfce = nfe.infNFe.total.ICMSTot.vNF.ToString("0.00").Replace(',', '.');
                var digVal = QrCodeUtils.ObterHexDeString(nfe.Signature.SignedInfo.Reference.DigestValue);
                dadosBase = string.Concat(chave, pipe, (int)eVersaoQrCode.QrCodeVersao2, pipe, ambiente, pipe, diaEmi, pipe, valorNfce, pipe, digVal, pipe, idCsc);
            }
            else
            {
                dadosBase = string.Concat(chave, pipe, (int)eVersaoQrCode.QrCodeVersao2, pipe, ambiente, pipe, idCsc);
            }

            var dadosSha1 = string.Concat(dadosBase, csc);
            var sh1 = QrCodeUtils.ObterHexSha1DeString(dadosSha1);

            return string.Concat(url, dadosBase, pipe, sh1);
        }

        public static string ObterUrlQrCode(NFe nfe, eVersaoQrCode versaoQrCode, string cIdToken, string csc)
        {
            Func<string, string> msgErro = parametro => $"O {parametro} não foi informado!";

            if (string.IsNullOrEmpty(cIdToken))
                throw new ArgumentNullException(nameof(cIdToken), msgErro("token"));

            if (string.IsNullOrEmpty(csc))
                throw new ArgumentNullException(nameof(cIdToken), msgErro("CSC"));

            return ObterUrlQrCode2(nfe, cIdToken, csc);
        }
    }
}

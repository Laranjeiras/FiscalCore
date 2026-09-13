using System;
using FiscalCore.Configuracoes;
using FiscalCore.Danfe.NFCe.Nativo;
using FiscalCore.NotaFiscal;
using FiscalCore.Tipos;

namespace FiscalCore.Danfe.NFCe
{
    /// <summary>
    /// Cálculo da URL do QR-Code e formatação da chave de acesso da NFC-e. Funções puras,
    /// sem dependência do contexto de desenho.
    /// </summary>
    internal static class NfceQrCode
    {
        // CSC (Código de Segurança do Contribuinte) não participa da autorização da NFe: o
        // envio ao SEFAZ usa só o XML assinado com o certificado digital, e o SEFAZ nunca vê
        // o CSC nesse momento — por isso uma NFCe pode autorizar normalmente com o protocolo
        // certo mesmo com o CSC errado configurado.
        // O CSC só entra aqui: o QR-Code do DANFE é calculado localmente (sem chamada ao
        // SEFAZ), concatenando chave+CSC e gerando um hash SHA1 para montar a URL. Se o valor
        // configurado não for o token CSC real (ex.: um GUID por engano), o Convert.ToInt16
        // abaixo estoura FormatException e a impressão falha — a nota já autorizada continua
        // válida, só o QR-Code impresso é que fica comprometido.
        public static string ObterUrlQrCode(NFe nfe, IConfiguracaoDanfe configDanfe, string csc)
        {
            var url = configDanfe.NFCeUrlConsultaQrCodeSefaz;

            const string pipe = "|";

            //Chave de Acesso da NFC-e
            var chave = new string(nfe.infNFe.Id.AsSpan(3));

            //Identificação do Ambiente (1 – Produção, 2 – Homologação)
            var ambiente = (int)nfe.infNFe.ide.tpAmb;

            //Identificador do CSC (Código de Segurança do Contribuinte no Banco de Dados da SEFAZ). Informar sem os zeros não significativos
            var idCsc = Convert.ToInt16(csc);

            string dadosBase;

            if (nfe.infNFe.ide.tpEmis == eTipoEmissao.OffLine)
            {
                var diaEmi = nfe.infNFe.ide.dhEmi.Day.ToString("D2");
                var valorNfce = nfe.infNFe.total.ICMSTot.vNF.ToString("0.00").Replace(',', '.');
                var digVal = Conversor.ObterHexDeString(nfe.Signature.SignedInfo.Reference.DigestValue);
                dadosBase = string.Concat(chave, pipe, 2, pipe, ambiente, pipe, diaEmi, pipe, valorNfce, pipe, digVal, pipe, idCsc);
            }
            else
            {
                dadosBase = string.Concat(chave, pipe, 2, pipe, ambiente, pipe, idCsc);
            }

            var dadosSha1 = string.Concat(dadosBase, csc);
            var sh1 = Conversor.ObterHexSha1DeString(dadosSha1);

            return string.Concat(url, dadosBase, pipe, sh1);
        }

        public static string FormatarChaveAcesso(NFe nfe)
        {
            string chaveAcesso = new string(nfe.infNFe.Id.AsSpan(3));
            string novaChave = string.Empty;
            int contaChaveAcesso = 0;

            foreach (char c in chaveAcesso)
            {
                contaChaveAcesso++;
                novaChave += c;

                if (contaChaveAcesso == 4)
                {
                    novaChave += " ";
                    contaChaveAcesso = 0;
                }
            }
            return novaChave;
        }
    }
}

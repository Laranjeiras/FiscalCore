using System.Text;
using FiscalCore.NotaFiscal;
using FiscalCore.NotaFiscal.Informacoes.Destinatario;

namespace FiscalCore.Danfe.NFCe
{
    /// <summary>
    /// Composição das mensagens textuais do cupom NFC-e. Funções puras sobre os dados da NFe,
    /// sem dependência do contexto de desenho (Graphics) nem do cursor de layout.
    /// </summary>
    internal static class DanfeNfceTextos
    {
        public static string MontarMensagemEnderecoEmitente(NFe nfe)
        {
            var enderEmit = nfe.infNFe.emit.enderEmit;

            string foneEmit = string.Empty;

            if (enderEmit.fone != null)
            {
                foneEmit = $"\nFONE: {enderEmit.fone}";
            }


            StringBuilder enderecoEmitenteBuilder = new StringBuilder();
            enderecoEmitenteBuilder.Append(enderEmit.xLgr);
            enderecoEmitenteBuilder.Append(" ");

            if (string.IsNullOrEmpty(enderEmit.nro))
            {
                enderecoEmitenteBuilder.Append("S/N, ");
            }

            if (!string.IsNullOrEmpty(enderEmit.nro))
            {
                enderecoEmitenteBuilder.Append(enderEmit.nro);
                enderecoEmitenteBuilder.Append(", ");
            }
            enderecoEmitenteBuilder.Append("\n");
            enderecoEmitenteBuilder.Append(enderEmit.xBairro);
            enderecoEmitenteBuilder.Append(", ");
            enderecoEmitenteBuilder.Append(enderEmit.xMun);
            enderecoEmitenteBuilder.Append(", ");
            enderecoEmitenteBuilder.Append(enderEmit.UF);
            enderecoEmitenteBuilder.Append(foneEmit);

            return enderecoEmitenteBuilder.ToString();
        }

        public static string MontarMensagemRazaoSocial(NFe nfe)
        {
            var emitente = nfe.infNFe.emit;
            return string.IsNullOrEmpty(emitente.xFant) ? emitente.xNome : emitente.xFant;
        }

        public static string MontarMensagemCpfCnpjIE(NFe nfe)
        {
            var emitente = nfe.infNFe.emit;
            return string.IsNullOrEmpty(emitente.CNPJ) ? $"CPF: {emitente.CPF}" : $"CNPJ: {emitente.CNPJ}    IE: {emitente.IE}";
        }

        public static string MontarMensagemDadosNfce(NFe nfe)
        {
            StringBuilder mensagem = new StringBuilder("NFC-e nº ");
            mensagem.Append(nfe.infNFe.ide.nNF.ToString("D9"));
            mensagem.Append(" Série ");
            mensagem.Append(nfe.infNFe.ide.serie.ToString("D3"));
            mensagem.Append(" ");
            mensagem.Append(nfe.infNFe.ide.dhEmi.ToString("G"));
            mensagem.Append(" - ");
            mensagem.Append("Via consumidor");

            return mensagem.ToString();
        }

        public static string MontarMensagemConsumidor(dest dest)
        {
            StringBuilder mensagem = new StringBuilder("CONSUMIDOR ");

            if (dest == null || (string.IsNullOrEmpty(dest.CPF) && string.IsNullOrEmpty(dest.CNPJ)))
            {
                mensagem.Append("NÃO IDENTIFICADO");
                return mensagem.ToString();
            }

            if (!string.IsNullOrEmpty(dest.idEstrangeiro))
            {
                mensagem.Append("Id ");
                mensagem.Append(dest.idEstrangeiro);
            }

            if (!string.IsNullOrEmpty(dest.CPF))
            {
                mensagem.Append("CPF ");
                mensagem.Append(dest.CPF);
            }

            if (!string.IsNullOrEmpty(dest.CNPJ))
            {
                mensagem.Append("CNPJ ");
                mensagem.Append(dest.CNPJ);
            }

            if (!string.IsNullOrEmpty(dest.xNome))
            {
                mensagem.Append(" ");
                mensagem.Append(dest.xNome);
            }

            enderDest enderecoDest = dest.enderDest;

            if (enderecoDest == null) return mensagem.ToString().Replace(", ,", ", ");

            string rua = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xLgr))
                rua = enderecoDest.xLgr;

            string numero = "S/N";
            if (!string.IsNullOrEmpty(enderecoDest.nro))
                numero = enderecoDest.nro;

            string bairro = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xBairro))
                bairro = enderecoDest.xBairro;

            string cidade = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xMun))
                bairro = enderecoDest.xMun;

            string siglaUf = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.UF))
                bairro = enderecoDest.UF;

            if (string.IsNullOrEmpty(rua)) return mensagem.ToString();
            mensagem.Append(" - ");
            mensagem.Append(rua);
            mensagem.Append(", ");
            mensagem.Append(numero);
            mensagem.Append(", ");
            mensagem.Append(bairro);
            mensagem.Append(", ");
            mensagem.Append(cidade);
            mensagem.Append(" - ");
            mensagem.Append(siglaUf);

            return mensagem.ToString().Replace(", ,", ", ");
        }
    }
}

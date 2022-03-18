using System;
using System.IO;
using System.Threading.Tasks;
using FiscalCore.NotaFiscal;
using FiscalCore.Tipos;
using FiscalCore.Utils;

namespace FiscalCore.Extensions
{
    public static class NFeExtension
    {
        public static eModeloDocumento ModeloDocumento(this string modelo)
        {
            if (modelo != "55" && modelo != "65")
                throw new Exception("Os modelos válidos são 55 ou 65");
            var modeloDoc = (eModeloDocumento)Enum.Parse(typeof(eModeloDocumento), modelo);
            return modeloDoc;
        }

        public static async Task<string> SalvarArquivoAsync(this NFe nfe, string nomeArquivo, string diretorio)
        {
            if (nomeArquivo is null)
                throw new ArgumentNullException(nameof(nomeArquivo));

            if (string.IsNullOrEmpty(diretorio))
                diretorio = Path.GetTempPath();

            var str = XmlUtils.ClasseParaXmlString<NFe>(nfe);
            return await Arquivo.SalvarArquivoAsync(diretorio, nomeArquivo, str);
        }
    }
}

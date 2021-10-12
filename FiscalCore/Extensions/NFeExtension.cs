using System;
using System.IO;
using System.Threading.Tasks;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Tipos;
using FiscalCore.Utils;

namespace FiscalCore.Extensions
{
    public static class NFeExtension
    {

        public static eModeloDocumento Parse(this DFeBR.EmissorNFe.Utilidade.Tipos.ModeloDocumento origem)
        {
            return (eModeloDocumento) (int)origem;
        }

        public static eModeloDocumento ModeloDocumento(this string modelo)
        {
            if (modelo != "55" && modelo != "65")
                throw new Exception("Os modelos válidos são 55 ou 65");
            var modeloDoc = (eModeloDocumento)Enum.Parse(typeof(eModeloDocumento), modelo);
            return modeloDoc;
        }

        public static async Task<string> SalvarArquivoAsync(this NFe nfe)
        {
            var caminho = Path.GetTempFileName();
            var nome = Path.GetFileName(caminho);
            var diretorio = Path.GetDirectoryName(caminho);

            return await nfe.SalvarArquivoAsync(nome, diretorio);
        }

        public static async Task<string> SalvarArquivoAsync(this NFe nfe, string arquivo, string diretorio = null)
        {
            if (arquivo is null)
                throw new ArgumentNullException(nameof(arquivo));
            
            if (string.IsNullOrEmpty(diretorio))
                diretorio = Path.GetTempPath();

            var str = XmlUtils.ClasseParaXmlString<NFe>(nfe);
            return await Arquivo.SalvarArquivoAsync(diretorio, arquivo, str);
        }
    }
}

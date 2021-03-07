using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.Autorizacao;
using FiscalCore.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System;
using FiscalCore.Tipos;

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
    }
}

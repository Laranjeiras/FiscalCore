using FiscalCore.Enums;
using System;

namespace FiscalCore.Utils
{
    public class Conversor
    {
        public static eModeloDocumento ModeloDocumento(string modelo)
        {
            if (modelo != "55" && modelo != "65")
                throw new Exception("Os modelos válidos são 55 ou 65");
            var modeloDoc = (eModeloDocumento)Enum.Parse(typeof(eModeloDocumento), modelo);
            return modeloDoc;
        }
    }
}

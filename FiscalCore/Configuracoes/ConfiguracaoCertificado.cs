using FiscalCore.Tipos;
using System;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoCertificado
    {
        public ConfiguracaoCertificado()
        {
        }

        public ConfiguracaoCertificado(eTipoCertificado tipoCertificado, string serial)
        {
            if (string.IsNullOrEmpty(serial))
                throw new Exception("Serial não informado");

            TipoCertificado = tipoCertificado;
            Serial = serial;
        }

        public ConfiguracaoCertificado(string arquivoCertificado, string senha = null)
        {
            if (!File.Exists(arquivoCertificado))
                throw new Exception("Arquivo do certificado não encontrado");

            TipoCertificado = eTipoCertificado.A1Arquivo;
            ArquivoCertificado = arquivoCertificado;
            Senha = senha;
        }

        public eTipoCertificado TipoCertificado { get; set; }
        public string ArquivoCertificado { get; set; }
        public string Serial { get; set; }
        public string Senha { get; set; }
        public string SignatureMethodSignedXml { get; set; } = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public string DigestMethodReference { get; set; } = "http://www.w3.org/2000/09/xmldsig#sha1";
    }
}

using FiscalCore.Enums;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoCertificado
    {
        public ConfiguracaoCertificado()
        {

        }

        public ConfiguracaoCertificado(eTipoCertificado tipoCertificado, string serial)
        {
            TipoCertificado = tipoCertificado;
            Serial = serial; 
        }

        public eTipoCertificado TipoCertificado { get; set; }
        public string Serial { get; set; }
        public byte[] ArrayBytesArquivo { get; set; }
        public string Arquivo { get; set; }
        public string Senha { get; set; }
        public string CacheId { get; set; }
        public bool ManterDadosEmCache { get; set; }
        public string SignatureMethodSignedXml { get; set; } = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public string DigestMethodReference { get; set; } = "http://www.w3.org/2000/09/xmldsig#sha1";
    }
}

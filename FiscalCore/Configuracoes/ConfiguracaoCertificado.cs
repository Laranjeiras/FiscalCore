using FiscalCore.Enums;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoCertificado
    {
        public ConfiguracaoCertificado(TipoCertificado tipoCertificado, string serial)
        {
            TipoCertificado = tipoCertificado;
            Serial = serial;
        }

        public ConfiguracaoCertificado(string arquivoCertificado, string senha = null)
        {
            TipoCertificado = TipoCertificado.A1Arquivo;
            ArquivoCertificado = arquivoCertificado;
            Senha = senha;
        }

        public TipoCertificado TipoCertificado { get; protected set; }
        public string ArquivoCertificado { get; protected set; }
        public string Serial { get; protected set; }
        public string Senha { get; protected set; }
        public string SignatureMethodSignedXml { get; protected set; } = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public string DigestMethodReference { get; protected set; } = "http://www.w3.org/2000/09/xmldsig#sha1";
    }
}

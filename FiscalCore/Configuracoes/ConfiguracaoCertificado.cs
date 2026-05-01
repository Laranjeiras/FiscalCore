using FiscalCore.Tipos;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoCertificado
    {
        public ConfiguracaoCertificado()
        {
        }

        public ConfiguracaoCertificado(X509Certificate2 certificado, string? senha = null)
        {
            _certificado = certificado;
            Senha = senha;
        }

        public ConfiguracaoCertificado(eTipoCertificado tipoCertificado, string serial)
        {
            if (string.IsNullOrEmpty(serial))
                throw new Exception("Serial não informado");

            TipoCertificado = tipoCertificado;
            Serial = serial;
        }

        public ConfiguracaoCertificado(string arquivoCertificado, string senha)
        {
            if (!File.Exists(arquivoCertificado))
                throw new Exception("Arquivo do certificado não encontrado");

            TipoCertificado = eTipoCertificado.A1Arquivo;
            ArquivoCertificado = arquivoCertificado;
            Senha = senha;
        }

        public ConfiguracaoCertificado(byte[] bytes, string senha)
        {
            TipoCertificado = eTipoCertificado.A1Arquivo;
            _certificado = Utils.Certificado.ObterDeArquivo(bytes, senha);
        }

        public eTipoCertificado TipoCertificado { get; set; }
        public string? ArquivoCertificado { get; set; }
        public string? Serial { get; set; }
        public string? Senha { get; set; }
        public string SignatureMethodSignedXml { get; set; } = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public string DigestMethodReference { get; set; } = "http://www.w3.org/2000/09/xmldsig#sha1";

        private X509Certificate2? _certificado;
        public X509Certificate2 Certificado => _certificado ?? Obter();

        private X509Certificate2 Obter()
        {
            switch (TipoCertificado)
            {
                case eTipoCertificado.A1Repositorio:
                    return Utils.Certificado.ObterDoRepositorio(Serial!);
                case eTipoCertificado.A1Arquivo:
                    return Utils.Certificado.ObterDeArquivo(ArquivoCertificado!, Senha!);
                case eTipoCertificado.A3:
                    return Utils.Certificado.ObterDoRepositorio(Serial!);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

namespace FiscalCore.Tipos
{
    /// <summary>
    /// Comprimentos e posições fixados pelo MOC (Manual de Orientação do Contribuinte).
    /// Valores definidos pela SEFAZ — não são parâmetros de configuração.
    /// </summary>
    public static class LayoutFiscal
    {
        /// <summary>Chave de acesso da NF-e/NFC-e (campo B-01 a B-23).</summary>
        public const int TamanhoChaveAcesso = 44;

        /// <summary>Número do protocolo de autorização.</summary>
        public const int TamanhoProtocolo = 15;

        /// <summary>NSU da Distribuição de DFe.</summary>
        public const int TamanhoNsu = 15;

        /// <summary>CNPJ, numérico ou alfanumérico (IN RFB nº 2.119/2022).</summary>
        public const int TamanhoCnpj = 14;

        /// <summary>CPF do emitente pessoa física.</summary>
        public const int TamanhoCpf = 11;

        /// <summary>Código numérico que compõe a chave de acesso (cNF).</summary>
        public const int TamanhoCnf = 8;

        /// <summary>Mínimo para justificativa de evento (cancelamento, manifestação).</summary>
        public const int JustificativaMinima = 15;

        /// <summary>Máximo para justificativa de evento.</summary>
        public const int JustificativaMaxima = 255;

        /// <summary>Mínimo para o texto de correção da CC-e.</summary>
        public const int CorrecaoMinima = 15;

        /// <summary>Máximo para o texto de correção da CC-e.</summary>
        public const int CorrecaoMaxima = 1000;

        /// <summary>Eventos por lote no envEvento.</summary>
        public const int MaximoEventosPorLote = 20;

        /// <summary>Dígitos do nSeqEvento no Id do evento.</summary>
        public const int TamanhoSequenciaEvento = 2;

        /// <summary>Dígitos da série na inutilização.</summary>
        public const int TamanhoSerie = 3;

        /// <summary>Dígitos do número da NF na inutilização.</summary>
        public const int TamanhoNumeroNF = 9;

        /// <summary>Offsets de parsing da chave de acesso, conforme layout do MOC.</summary>
        public static class PosicaoChave
        {
            public const int Uf = 0;
            public const int UfTamanho = 2;

            public const int AnoMesEmissao = 2;
            public const int AnoMesEmissaoTamanho = 4;

            public const int Cnpj = 6;
            public const int CnpjTamanho = 14;

            public const int Modelo = 20;
            public const int ModeloTamanho = 2;

            public const int Serie = 22;
            public const int SerieTamanho = 3;

            public const int Numero = 25;
            public const int NumeroTamanho = 9;

            public const int TipoEmissao = 34;
            public const int TipoEmissaoTamanho = 1;

            public const int Cnf = 35;
            public const int CnfTamanho = 8;

            public const int DigitoVerificador = 43;
            public const int DigitoVerificadorTamanho = 1;
        }
    }
}

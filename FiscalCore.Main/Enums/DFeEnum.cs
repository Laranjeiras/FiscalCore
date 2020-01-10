using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Main.Enums
{
    /// <summary>
    /// Código do Modelo do Documento Fiscal
    /// </summary>
    public enum eModeloDocumento
    {
        [Description("NFe")]
        [XmlEnum("55")]
        NFe = 55,
        [Description("NFCe")]
        [XmlEnum("65")]
        NFCe = 65
    }

    /// <summary>
    //      1=Empresa Emitente; 
    //      2=Empresa Destinatária;
    //      3=Empresa; 
    //      5=Fisco; 
    //      6=RFB; 
    //      9=Outros Órgãos
    /// </summary>
    public enum eTipoAutor
    {
        [XmlEnum("1")] 
        EmpresaEmitente = 1,
        [XmlEnum("2")] 
        EmpresaDestinataria = 2,
        [XmlEnum("3")] 
        Empresa = 3,
        [XmlEnum("5")] 
        Fisco = 5,
        [XmlEnum("6")] 
        RFB = 6,
        [XmlEnum("9")] 
        OutrosOrgaos = 9
    }

    public enum eTipoAmbiente
    {
        [XmlEnum("1")]
        [Description("Produção")]
        Producao = 1,

        [XmlEnum("2")]
        [Description("Homologação")]
        Homologacao = 2
    }

    public enum eNFeTipoEvento
    {
        [Description("Carta de Correção")]
        [XmlEnum("110110")]
        NfeCartaCorrecao = 110110,

        [Description("EPEC")]
        [XmlEnum("110140")]
        NfceEpec = 110140,

        [Description("Cancelamento")]
        [XmlEnum("110111")]
        NfeCancelamento = 110111,

        [Description("Cancelamento por substituicao")]
        [XmlEnum("110112")]
        NfeCancelamentoSubstituicao = 110112,
    }

    public enum eCRT
    {
        [Description("Simples Nacional")]
        [XmlEnum("1")] 
        SimplesNacional = 1,
        [Description("Simples Nacional – excesso de sublimite de receita bruta")]
        [XmlEnum("2")]
        SimplesNacionalExcessoSublimite = 2,
        [Description("Regime Normal")]
        [XmlEnum("3")] 
        RegimeNormal = 3
    }

    public enum eVersaoServico
    {
        [XmlEnum("1.00")]
        [Description("1.00")]
        Versao100 = 100,
        [Description("2.00")]
        [XmlEnum("2.00")]
        Versao200 = 200,
        [Description("3.10")]
        [XmlEnum("3.10")]
        Versao310 = 310,
        [Description("4.00")]
        [XmlEnum("4.00")]
        Versao400 = 400
    }

    /// <summary>
    ///     Forma de emissão da NF-e
    ///     <para>1 - Emissão normal (não em contingência)</para>
    ///     <para>2 - Contingência FS-IA, com impressão do DANFE em formulário de segurança</para>
    ///     <para>3 - Contingência SCAN (Sistema de Contingência do Ambiente Nacional)</para>
    ///     <para>4 - Contingência DPEC (Declaração Prévia da Emissão em Contingência)</para>
    ///     <para>5 - Contingência FS-DA, com impressão do DANFE em formulário de segurança</para>
    ///     <para>6 - Contingência SVC-AN (SEFAZ Virtual de Contingência do AN)</para>
    ///     <para>7 - Contingência SVC-RS (SEFAZ Virtual de Contingência do RS)</para>
    ///     <para>9 - Contingência off-line da NFC-e</para>
    ///     <para>Nota: Para a NFC-e somente estão disponíveis e são válidas as opções de contingência 5 e 9</para>
    /// </summary>
    public enum eTipoEmissao
    {
        [XmlEnum("1")]
        [Description("Normal")]
        Normal = 1,

        [XmlEnum("2")]
        [Description("Contingência FS-IA")]
        FSIA = 2,

        [XmlEnum("3")]
        [Description("Contingência SCAN")]
        SCAN = 3,

        [XmlEnum("4")]
        [Description("Contingência DPEC")]
        EPEC = 4,

        [XmlEnum("5")]
        [Description("Contingência FS-DA")]
        FSDA = 5,

        [XmlEnum("6")]
        [Description("Contingência SVC-AN")]
        SVCAN = 6,

        [XmlEnum("7")]
        [Description("Contingência SVC-RS")]
        SVCRS = 7,

        [XmlEnum("9")]
        [Description("Contingência off-line")]
        OffLine = 9
    }

    public enum eTipoImpressao
    {
        [XmlEnum("0")]
        [Description("Sem geração de DANFE")]
        SemDanfe = 0,

        [XmlEnum("1")]
        [Description("DANFE normal, Retrato")]
        NormalRetrato = 1,

        [XmlEnum("2")]
        [Description("DANFE normal, Paisagem")]
        NormalPaisagem = 2,

        [XmlEnum("3")]
        [Description("DANFE Simplificado")]
        Simplificado = 3,

        [XmlEnum("4")]
        [Description("DANFE NFC-e")]
        Nfce = 4,

        [XmlEnum("5")]
        [Description("DANFE NFC-e em mensagem eletrônica")]
        MensagemEletronica = 5
    }
}

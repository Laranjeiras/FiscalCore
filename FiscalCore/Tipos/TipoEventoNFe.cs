using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoEventoNFe
    {
        [Obsolete("Colocado apenas para ser usado em um ERP (banco de dados) legado (DFeEventos/TpEvento = 0)")]
        [Description("Nenhum evento informado")]
        [XmlEnum("0")]
        NenhumEventoInformado = 0,

        [Description("Carta de Correção")]
        [XmlEnum("110110")]
        NFeCartaCorrecao = 110110,

        [Description("Cancelamento")]
        [XmlEnum("110111")]
        NFeCancelamento = 110111,

        [Description("Cancelamento por substituicao")]
        [XmlEnum("110112")]
        NFeCancelamentoSubstituicao = 110112,

        [Description("Comprovante de Entrega da NF-e")]
        [XmlEnum("110130")]
        ComprovanteEntregaNFe = 110130,

        [Description("Cancelamento do Comprovante de Entrega da NF-e")]
        [XmlEnum("110131")]
        CancelamentoComprovanteEntregaNFe = 110131,

        [Description("EPEC")]
        [XmlEnum("110140")]
        NFceEpec = 110140,

        #region Manifestação do Detinatário
        [Description("Confirmação da Operação")]
        [XmlEnum("210200")]
        ConfirmacaoOperacao = 210200,

        [Description("Ciência da Operação")]
        [XmlEnum("210210")]
        CienciaOperacao = 210210,

        [Description("Desconhecimento da Operação")]
        [XmlEnum("210220")]
        DesconhecimentoOperacao = 210220,

        [Description("Operação não Realizada")]
        [XmlEnum("210240")]
        OperacaoNaoRealizada = 210240,
        #endregion

        [Description("Registro de Passagem Automatico MDFe")]
        [XmlEnum("610552")]
        RegistroPassagemAutomaticoMDFe = 610552,

        [Description("Eventos de MDF-e Autorizado")]
        [XmlEnum("610610")]
        EventosMDFeAutorizado = 610610,

        [Description("Eventos de Registro de Passagem Autorização")]
        [XmlEnum("610500")]
        EventosRegistroPassagemAutorizacao = 610500,

        [Description("Eventos de Registro de Cancelamento de Passagem")]
        [XmlEnum("610501")]
        EventosRegistroCancelamentoPassagem = 610501,

        [Description("Eventos de Registro de Passagem de NF-e MDF-e")]
        [XmlEnum("610510")]
        EventosRegistroPassagemNFeMDFe = 610510,

        [Description("Eventos de Registro de Passagem de NF-e CT-e")]
        [XmlEnum("610514")]
        EventosRegistroPassagemNFeCTe = 610514,

        [Description("Eventos de Registro de Passagem Automática")]
        [XmlEnum("610550")]
        EventosRegistroPassagemAutomatica = 610550,

        [Description("Eventos de Registro de Passagem Automática MDF-e CT-e")]
        [XmlEnum("610554")]
        EventosRegistroPassagemAutomaticaMDFeCTe = 610554,

        [Description("Eventos de Registro de Autorização CT-e NF-e")]
        [XmlEnum("610600")]
        EventosRegistroAutorizaçãoCTeNFe = 610600,

        [Description("Eventos de Registro de Cancelamento de CT-e NF-e")]
        [XmlEnum("610601")]
        EventosRegistroCancelamentoCTeNFe = 610601,

        [Description("Eventos de MDF-e Cancelado")]
        [XmlEnum("610611")]
        EventosMDFeCancelado = 610611,

        [Description("Eventos de MDF-e Autorizado CT-e")]
        [XmlEnum("610614")]
        EventosMDFeAutorizadoCTe = 610614,

        [Description("Eventos de Cancelamento de MDF-e CT-e")]
        [XmlEnum("610615")]
        EventosCancelamentoMDFeCTe = 610615
    }
}

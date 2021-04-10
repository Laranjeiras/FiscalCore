using System.ComponentModel;

namespace FiscalCore.Tipos
{
    public enum eTipoServico
    {
        [Description("AutorizarNFe")]
        AutorizarNFe = 1,
        [Description("Cancelar NFe")]
        CancelarNFe = 2,
        [Description("Inutilização NFe")]
        InutilizacaoNFe = 3,
        [Description("Consultar Situação NFe")]
        ConsultaSituacaoNFe = 4,
        [Description("Carta Correção")]
        CartaCorrecao = 5,
        [Description("Manifestação do Destinatário")]
        ManifestacaoDestinatario = 6,
        [Description("Distribuição DFe")]
        NFeDistribuicaoDFe = 7,
    }
}

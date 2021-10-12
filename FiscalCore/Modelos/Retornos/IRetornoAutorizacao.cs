namespace FiscalCore.Modelos.Retornos
{
    public interface IRetornoAutorizacao
    {
        retEnviNFe Retorno { get; set; }
        bool Autorizado { get; }
    }
}

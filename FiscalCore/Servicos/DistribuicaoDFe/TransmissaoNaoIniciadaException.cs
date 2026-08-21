using System;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Indica que a falha ocorreu antes de qualquer envio HTTP à SEFAZ.</summary>
public sealed class TransmissaoNaoIniciadaException : Exception
{
    public TransmissaoNaoIniciadaException(string mensagem, Exception innerException)
        : base(mensagem, innerException)
    {
    }
}

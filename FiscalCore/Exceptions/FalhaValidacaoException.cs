using System;

namespace FiscalCore.Exceptions
{
    public class FalhaValidacaoException : Exception
    {
        public FalhaValidacaoException(string message) : base(message)
        {
        }

        public FalhaValidacaoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

using System;

namespace FiscalCore.Exceptions
{
    public class FalhaValidacaoSchemaException : Exception
    {
        public FalhaValidacaoSchemaException(string message) : base(message)
        {
        }

        public FalhaValidacaoSchemaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

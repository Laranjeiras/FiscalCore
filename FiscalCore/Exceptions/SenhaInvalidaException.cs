using System;

namespace FiscalCore.Exceptions
{
    public class SenhaInvalidaException : Exception
    {
        public SenhaInvalidaException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}

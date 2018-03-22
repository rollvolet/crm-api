using System;

namespace Rollvolet.CRM.Domain.Exceptions
{
    public class IllegalArgumentException : CodedException
    {
        public static string TITLE = "Illegal argument";

        public IllegalArgumentException() : base(string.Empty, TITLE, string.Empty) {}

        public IllegalArgumentException(string code, string message) : base(code, TITLE, message) {}

        public IllegalArgumentException(string code, string message, Exception innerException) : base(code, TITLE, message, innerException) { }
    }
}
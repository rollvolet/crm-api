using System;

namespace Rollvolet.CRM.Domain.Exceptions
{
    public class CodedException : Exception
    {
        public string Code { get; }
        public string Title { get; }

        public CodedException() : base() {}

        public CodedException(string code, string title, string message) : base(message)
        {
            Code = code;
            Title = title;
        }

        public CodedException(string code, string title, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
            Title = title;
        }
    }
}
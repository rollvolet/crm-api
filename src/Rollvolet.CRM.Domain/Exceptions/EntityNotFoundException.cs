using System;

namespace Rollvolet.CRM.Domain.Exceptions
{
    public class EntityNotFoundException : CodedException
    {
        public static string TITLE = "Entity not found";

        public EntityNotFoundException() : base(string.Empty, TITLE, string.Empty) {}

        public EntityNotFoundException(string code, string message) : base(code, TITLE, message) {}

        public EntityNotFoundException(string code, string message, Exception innerException) : base(code, TITLE, message, innerException) { }
    }
}
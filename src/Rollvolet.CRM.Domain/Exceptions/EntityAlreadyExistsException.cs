using System;

namespace Rollvolet.CRM.Domain.Exceptions
{
    public class EntityAlreadyExistsException : CodedException
    {
        public static string TITLE = "Entity already exists";

        public EntityAlreadyExistsException() : base(string.Empty, TITLE, string.Empty) {}

        public EntityAlreadyExistsException(string code, string message) : base(code, TITLE, message) {}

        public EntityAlreadyExistsException(string code, string message, Exception innerException) : base(code, TITLE, message, innerException) { }
    }
}
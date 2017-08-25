using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ErrorResponse : JsonApiResponse
    {
        public IEnumerable<object>  Errors { get; set; }
    }
}
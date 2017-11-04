using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ErrorResponse
    {
        public object Meta { get; set; }
        public object Links { get; set; }
        public IEnumerable<object>  Errors { get; set; }
    }
}
using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public abstract class JsonApiResponse
    {
        public object Meta { get; set; }
        public object Links { get; set; }
    }
}
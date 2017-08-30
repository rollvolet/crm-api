using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceResponse : JsonApiResponse
    {
        public object Data { get; set; }
        public IEnumerable<object> Included { get; set; } = new List<object>();
    }
}
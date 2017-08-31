using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceResponse : JsonApiResponse
    {
        public object Data { get; set; } // data may be an Resource or an IEnumerable<Resource>
        public IEnumerable<Resource> Included { get; set; } = new List<Resource>();
    }
}
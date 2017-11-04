using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceResponse
    {
        public object Meta { get; set; }
        public object Links { get; set; }
        public object Data { get; set; } // data may be a Resource or an IEnumerable<Resource>
        public IEnumerable<IResource> Included { get; set; }
    }
}
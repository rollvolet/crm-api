using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceResponse
    {
        // We're using object as datatype on purpose here
        // so System.Text.Json uses the actual types to serialize instead of the derived types
        public object Meta { get; set; }
        public object Links { get; set; }
        public object Data { get; set; } // data may be a Resource or an IEnumerable<Resource>
        public IEnumerable<object> Included { get; set; } // IEnumarable<IResource>
    }
}
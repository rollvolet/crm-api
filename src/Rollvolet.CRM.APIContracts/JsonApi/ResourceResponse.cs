using System.Collections.Generic;
using Rollvolet.CRM.APIContracts.JsonApi.Interfaces;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceResponse : JsonApiResponse
    {
        public object Data { get; set; } // data may be an IResource or an IEnumerable<IResource>
        public IEnumerable<IResource<IDto>> Included { get; set; } = new List<IResource<IDto>>();
    }
}
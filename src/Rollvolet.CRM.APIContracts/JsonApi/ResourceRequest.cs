using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ResourceRequest<T> where T : IResource
    {
        public T Data { get; set; }
    }
}
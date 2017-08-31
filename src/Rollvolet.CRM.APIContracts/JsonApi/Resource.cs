using Rollvolet.CRM.APIContracts.JsonApi.Interfaces;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Resource<T> : IResource<T> where T : IDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public T Attributes { get; set; }
        public object Relationships { get; set; }
    }
}
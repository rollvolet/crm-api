namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Resource<T>
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public T Attributes { get; set; }
        public object Relationships { get; set; }
    }
}
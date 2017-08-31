namespace Rollvolet.CRM.APIContracts.JsonApi.Interfaces
{
    public interface IResource<T> where T : IDto
    {
        string Id { get; set; }
        string Type { get; set; }
        T Attributes { get; set; }
        object Relationships { get; set; }
    }
}
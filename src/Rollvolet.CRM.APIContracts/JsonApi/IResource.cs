namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public interface IResource
    {
        string Id { get; set; }
        string Type { get; set; }
    }
}
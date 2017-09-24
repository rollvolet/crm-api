namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Relationship
    {
        public Links Links { get; set; }
        public object Data { get; set; } // maybe a RelationResource or IEnumerable<RelationResource>
    }
}
namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Relationship
    {
        public Links Links { get; set; }
        public object Data { get; set; }  // data may be a RelationResource or an IEnumerable<RelationResource>
    }
}
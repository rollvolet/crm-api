namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class OneRelationship : IRelationship 
    {
        public Links Links { get; set; }
        public RelatedResource Data { get; set; }
    }
}
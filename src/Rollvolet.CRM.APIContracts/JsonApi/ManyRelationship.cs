using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class ManyRelationship : IRelationship 
    {
        public Links Links { get; set; }
        public IEnumerable<RelatedResource> Data { get; set; }
    }
}
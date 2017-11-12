using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Relationship : IRelationship 
    {
        public Links Links { get; set; }
    }
}
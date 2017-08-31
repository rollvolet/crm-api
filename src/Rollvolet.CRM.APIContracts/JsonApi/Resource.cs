using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public abstract class Resource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public object Attributes { get; set; }
        public IDictionary<string, Relationship> Relationships { get; set; }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() * 17 + this.Type.GetHashCode();
        }

        public override bool Equals(object obj) 
        {
            if (obj == null || GetType() != obj.GetType()) 
                return false;

            Resource r = (Resource)obj;
            return (Id == r.Id) && (Type == r.Type);
        }
    }
}
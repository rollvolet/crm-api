using System.Collections.Generic;

namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public abstract class Resource<A, R> : IResource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public A Attributes { get; set; }
        public R Relationships { get; set; }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() * 17 + this.Type.GetHashCode();
        }

        public override bool Equals(object obj) 
        {
            if (obj == null || GetType() != obj.GetType()) 
                return false;

            Resource<A,R> r = (Resource<A,R>)obj;
            return (Id == r.Id) && (Type == r.Type);
        }
    }
}
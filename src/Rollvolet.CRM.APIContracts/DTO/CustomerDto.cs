using System.Collections.Generic;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CustomerDto : Resource
    {
        public new AttributesDto Attributes { get; set; }

        public class AttributesDto {
            public int DataId { get; set; }
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
        }
    }
}
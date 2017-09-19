using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class TelephoneDto : Resource
    {        
        public new AttributesDto Attributes { get; set; }
        public class AttributesDto {
            public string Area { get; set; }
            public string Number { get; set; }
            public string Memo { get; set; }
            public int Order { get; set; }
        }
  }
}
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class PostalCodeDto : Resource
    {        
        public new AttributesDto Attributes { get; set; }
        public class AttributesDto {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Distance { get; set; }
        }
  }
}
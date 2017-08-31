using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class BuildingDto : Resource
    {
        public new AttributesDto Attributes { get; set; }
        
        public class AttributesDto {
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public int RelativeId { get; set; }
            public int CustomerId { get; set; }
        }
  }
}
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class HonorificPrefixDto : Resource
    {        
        public new AttributesDto Attributes { get; set; }
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class HonorificPrefixDto : Resource<HonorificPrefixDto.AttributesDto, EmptyRelationshipsDto>
    {        
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
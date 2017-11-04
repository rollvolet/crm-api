using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class TelephoneTypeDto : Resource<TelephoneTypeDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
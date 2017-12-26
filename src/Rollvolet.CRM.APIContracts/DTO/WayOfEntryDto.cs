using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class WayOfEntryDto : Resource<WayOfEntryDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
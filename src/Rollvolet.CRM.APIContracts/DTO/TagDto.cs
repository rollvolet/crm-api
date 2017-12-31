using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class TagDto : Resource<TagDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
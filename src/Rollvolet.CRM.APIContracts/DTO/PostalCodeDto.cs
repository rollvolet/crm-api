using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class PostalCodeDto : Resource<PostalCodeDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Distance { get; set; }
        }
  }
}
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class LanguageDto : Resource<LanguageDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Code { get; set; }
            public string Name { get; set; }
        }
  }
}
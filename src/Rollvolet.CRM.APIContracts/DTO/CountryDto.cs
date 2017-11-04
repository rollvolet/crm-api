using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CountryDto : Resource<CountryDto.AttributesDto, EmptyRelationshipsDto>
    {        
        public class AttributesDto {
            public string Code { get; set; }
            public string Name { get; set; }
        }
  }
}
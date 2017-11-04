using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class TelephoneDto : Resource<TelephoneDto.AttributesDto, TelephoneDto.RelationshipsDto>
    {        
        public new AttributesDto Attributes { get; set; }
        public class AttributesDto {
            public string Area { get; set; }
            public string Number { get; set; }
            public string Memo { get; set; }
            public int Order { get; set; }
        }

        public class RelationshipsDto
        {
            public OneRelationship TelephoneType { get; set; }
            public OneRelationship Country { get; set; }
        }
  }
}
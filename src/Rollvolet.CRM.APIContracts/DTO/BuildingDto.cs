using System;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class BuildingDto : Resource<BuildingDto.AttributesDto, BuildingDto.RelationshipsDto>
    {        
        public class AttributesDto {
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public int Number { get; set; }
            public int CustomerId { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public OneRelationship Customer { get; set; }
            public OneRelationship Country { get; set; }
            public OneRelationship Language { get; set; }
            public OneRelationship PostalCode { get; set; }
        }
  }
}
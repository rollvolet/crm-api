using System;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ProductDto : Resource<ProductDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
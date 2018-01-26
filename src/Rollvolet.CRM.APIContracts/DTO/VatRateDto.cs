using System;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class VatRateDto : Resource<VatRateDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto {
            public string Code { get; set; }
            public double Rate { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public DateTime ExpirationDate { get; set; }
        }
  }
}
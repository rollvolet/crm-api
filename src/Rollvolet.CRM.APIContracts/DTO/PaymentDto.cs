using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class PaymentDto : Resource<PaymentDto.AttributesDto, EmptyRelationshipsDto>
    {        
        public class AttributesDto {
            public string Name { get; set; }
        }
  }
}
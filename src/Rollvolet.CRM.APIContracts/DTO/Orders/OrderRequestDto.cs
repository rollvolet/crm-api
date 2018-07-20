using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderRequestDto : Resource<OrderAttributesDto, OrderRequestRelationshipsDto>
    {
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Customers
{
    public class CustomerDto : Resource<CustomerAttributesDto, CustomerRelationshipsDto>
    {
    }
}
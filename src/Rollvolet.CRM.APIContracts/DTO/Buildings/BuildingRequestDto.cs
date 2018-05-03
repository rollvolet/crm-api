using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Buildings
{
    public class BuildingRequestDto : Resource<BuildingAttributesDto, BuildingRequestRelationshipsDto>
    {
    }
}
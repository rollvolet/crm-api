using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.WorkingHours
{
    public class WorkingHourDto : Resource<WorkingHourAttributesDto, WorkingHourRelationshipsDto>
    {
    }
}
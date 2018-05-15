using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class WorkingHourDto : Resource<WorkingHourDto.AttributesDto, WorkingHourDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Invoice { get; set; }
            public IRelationship Employee { get; set; }
        }
    }
}
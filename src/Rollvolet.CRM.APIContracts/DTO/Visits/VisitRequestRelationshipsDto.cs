using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Visits
{
    public class VisitRequestRelationshipsDto
    {
        public OneRelationship Customer { get; set; }
        public OneRelationship Request { get; set; }
    }
}
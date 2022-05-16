using System;

namespace Rollvolet.CRM.APIContracts.DTO.Interventions
{
    public class InterventionAttributesDto
    {
        public DateTime? Date { get; set; }
        public DateTime? PlanningDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public float? NbOfPersons { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationReason { get; set; }
    }
}
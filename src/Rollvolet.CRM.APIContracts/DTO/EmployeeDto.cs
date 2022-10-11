using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class EmployeeDto : Resource<EmployeeDto.AttributesDto, EmployeeDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            public int Id { get; set; }
            public short Type { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Initials { get; set; }
            public string Comment { get; set; }
            public bool Active { get; set; }
            public string Function { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Interventions { get; set; }
        }
    }
}
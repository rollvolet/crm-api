namespace Rollvolet.CRM.APIContracts.DTO
{
    public class BuildingDto : CustomerEntityDto
    {
        public override string Type { get; set; } = "buildings";
        public int RelativeId { get; set; }
        public int CustomerId { get; set; }
  }
}
namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CustomerDto : CustomerEntityDto
    {
        public override string Type { get; set; } = "customers";
        public int DataId { get; set; }
  }
}
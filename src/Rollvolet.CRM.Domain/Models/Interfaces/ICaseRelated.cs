namespace Rollvolet.CRM.Domain.Models.Interfaces
{
    public interface ICaseRelated
    {
        Customer Customer { get; set; }
        Building Building { get; set; }
        Contact Contact { get; set; }
    }
}
using System.Collections.Generic;

namespace Rollvolet.CRM.DataProvider.Models.Interfaces
{
    public interface ICaseRelated
    {
        int? CustomerId { get; set; }
        int? RelativeBuildingId { get; set; }
        int? RelativeContactId { get; set; }
        
        Customer Customer { get; set; }
        Building Building { get; set; }
        Contact Contact { get; set; }        
    }
}
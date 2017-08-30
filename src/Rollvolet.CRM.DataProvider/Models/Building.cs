using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Building : CustomerRecord
    {
        [Column("ParentID")]
        public int CustomerId { get; set; }

        [Column("ID")]
        public int BuildingId { get; set; }
    }
}
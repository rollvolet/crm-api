using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public abstract class CustomerEntity
    {
        public int DataId { get; set; }

        [Column("Naam")]
        public string Name { get; set; } 

        [Column("Adres1")]
        public string Address1 { get; set; }
        [Column("Adres2")]
        public string Address2 { get; set; }
        [Column("Adres3")]
        public string Address3 { get; set; }
    }
}
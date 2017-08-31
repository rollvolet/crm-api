using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public abstract class CustomerRecord
    {
        [Column("DataID")]
        public int DataId { get; set; }
        [Column("ID")]
        public int AlternateId { get; set; }
        [Column("Naam")]
        public string Name { get; set; } 
        [Column("Adres1")]
        public string Address1 { get; set; }
        [Column("Adres2")]
        public string Address2 { get; set; }
        [Column("Adres3")]
        public string Address3 { get; set; }
        [Column("LandId")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
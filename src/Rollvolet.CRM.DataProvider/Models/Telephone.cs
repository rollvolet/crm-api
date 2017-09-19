using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Telephone
    {
        [Column("DataId")]
        public int CustomerId { get; set; }

        [Column("TelTypeId")]
        public int TelephoneTypeId { get; set; }
        
        [Column("LandId")]
        public int CountryId { get; set; }
        
        [Column("Zonenr")]
        public string Area { get; set; }

        [Column("Telnr")]
        public string Number { get; set; }

        [Column("TelMemo")]
        public string Memo { get; set; }

        [Column("Volgorde")]
        public short Order { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Country Country { get; set; }
        public TelephoneType TelephoneType { get; set; }
    }
  
}
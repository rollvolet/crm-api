using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Customer : CustomerRecord
    {
        [Column("Firma")]
        public bool IsCompany { get; set; }

        [Column("BTWNummer")]
        public string VatNumber { get; set; }


        // Include resources
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
    }
}
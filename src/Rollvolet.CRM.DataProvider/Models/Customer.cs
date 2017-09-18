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

        [Column("AanspreekID")]
        public int? HonorificPrefixId { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }
        
        [Column("email")]
        public string Email { get; set; }

        [Column("email2")]
        public string Email2 { get; set; }

        [Column("URL")]
        public string Url { get; set; }

        [Column("PrintPrefix")]
        public bool PrintPrefix { get; set; }

        [Column("PrintSuffix")]
        public bool PrintSuffix { get; set; }

        [Column("PrintVoor")]
        public bool PrintInFront { get; set; }
        
        [Column("Opmerking")]
        public string Comment { get; set; }


        // Include resources
        public HonorificPrefix HonorificPrefix { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
    }
}
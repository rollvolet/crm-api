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


        // Included resources
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
        public IEnumerable<Request> Requests { get; set; }
        public IEnumerable<Offer> Offers { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
        
        
        public Memo Memo { get; set; }
        public IEnumerable<CustomerTag> CustomerTags { get; set; }
    }
}
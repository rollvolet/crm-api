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


        // BE0664977164 => BE 0664.977.164
        public static string SerializeVatNumber(string vatNumber)
        {
            if (vatNumber != null)
            {
                var country = vatNumber.Substring(0, 2).ToUpper();
                var number = vatNumber.Substring(2);
                return $"{country} {number.Substring(0,4)}.{number.Substring(4,3)}.{number.Substring(7)}";
            }
            else
            {
                return null;
            }
        }
    }
}
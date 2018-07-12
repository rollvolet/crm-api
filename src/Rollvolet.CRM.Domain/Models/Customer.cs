using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rollvolet.CRM.Domain.Models
{
    public class Customer : CustomerEntity
    {
        private static Regex _digitsOnly = new Regex(@"[\D]");

        public int DataId { get; set; }
        public bool IsCompany { get; set; }
        public string VatNumber { get; set; }
        public string Memo { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
        public IEnumerable<Request> Requests { get; set; }
        public IEnumerable<Offer> Offers { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
        public IEnumerable<Tag> Tags { get; set; }


        public static string SerializeVatNumber(string vatNumber)
        {
            if (vatNumber != null)
            {
                var country = vatNumber.Substring(0, 2);
                var number = vatNumber.Substring(2);
                number = _digitsOnly.Replace(number, "");

                if (country.ToUpper() == "BE" && number.Length == 9)
                    number = $"0{number}";

                return $"{country}{number}";
            }
            else
            {
                return null;
            }
        }
    }
}
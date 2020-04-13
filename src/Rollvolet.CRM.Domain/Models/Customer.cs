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
        public IEnumerable<Tag> Tags { get; set; }

        public bool IsValidVatNumber()
        {
            return IsValidVatNumber(VatNumber);
        }

        public static string SerializeVatNumber(string vatNumber)
        {
            if (IsBelgianVatNumber(vatNumber))
            {
                var country = vatNumber.Substring(0, 2);
                var number = vatNumber.Substring(2);
                number = _digitsOnly.Replace(number, "");

                if (number.Length == 9)
                    number = $"0{number}";

                return $"{country}{number}";
            }
            else
            {
                return null;
            }
        }

        public static bool IsValidVatNumber(string vatNumber)
        {
            if (IsBelgianVatNumber(vatNumber))
            {
                var number = vatNumber.Substring(2);
                var firstPart = int.Parse(number.Substring(0, 8));
                var secondPart = int.Parse(number.Substring(8));
                var modulo = firstPart % 97;

                if (secondPart + modulo != 97)
                    return false;
            }
            return true;
        }

        public static bool IsBelgianVatNumber(string vatNumber)
        {
            return vatNumber != null && vatNumber.Length >= 2 && vatNumber.Substring(0, 2).ToUpper() == "BE";
        }
    }
}
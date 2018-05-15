using System.Text.RegularExpressions;

namespace Rollvolet.CRM.Domain.Models
{
    public class Telephone
    {
        private static Regex _digitsOnly = new Regex(@"[\D]");

        public string Id { get; set; }
        public string Area { get; set; }
        public string Number { get; set; }
        public string Memo { get; set; }
        public int? Order { get; set; }
        public Building Building { get; set; }
        public Customer Customer { get; set; }
        public Contact Contact { get; set; }
        public Country Country { get; set; }
        public TelephoneType TelephoneType { get; set; }

        public static string SerializeNumber(string number)
        {
            return number != null ? _digitsOnly.Replace(number, "") : null;
        }

        public static string SerializeArea(string area)
        {
            return area != null ? _digitsOnly.Replace(area, "") : null;
        }
    }
}
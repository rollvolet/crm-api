using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Customer : CustomerEntity
    {
        public int DataId { get; set; }
        public bool IsCompany { get; set; }
        public string VatNumber { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
        public IEnumerable<Request> Requests { get; set; }
    }
}
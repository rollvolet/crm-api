using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Customer : CustomerEntity
    {
        public int DataId { get; set; }
        public bool IsCompany { get; set; }
        public string VatNumber { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Url { get; set; }
        public bool PrintPrefix { get; set; }
        public bool PrintSuffix { get; set; }
        public bool PrintInFront { get; set; }
        public string Comment { get; set; }

        public HonorificPrefix HonorificPrefix { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
        public IEnumerable<Telephone> Telephones { get; set; }
    }
}
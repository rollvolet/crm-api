using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Customer : CustomerEntity
    {
        public int DataId { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Building> Buildings { get; set; }
    }
}
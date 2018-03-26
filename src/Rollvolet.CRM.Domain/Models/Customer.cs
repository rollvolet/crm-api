using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Customer : CustomerEntity
    {
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
    }
}
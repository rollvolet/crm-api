using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public abstract class CustomerEntity
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public HonorificPrefix HonorificPrefix { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }
        public Language Language { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public IEnumerable<Telephone> Telephones { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Url { get; set; }
        public bool PrintPrefix { get; set; }
        public bool PrintSuffix { get; set; }
        public bool PrintInFront { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<Request> Requests { get; set; }
        public IEnumerable<Intervention> Interventions { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}
using System;

namespace Rollvolet.CRM.Domain.Models
{
    public class VatRate
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public double Rate { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
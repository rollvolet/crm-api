using System;

namespace Rollvolet.CRM.APIContracts.DTO.Customers
{
    public class CustomerAttributesDto
    {
        public int DataId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
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
        public string Memo { get; set; }
        public DateTime Created { get; set; }
    }
}
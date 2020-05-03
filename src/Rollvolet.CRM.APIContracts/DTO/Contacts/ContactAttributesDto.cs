using System;

namespace Rollvolet.CRM.APIContracts.DTO.Contacts
{
    public class ContactAttributesDto
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Url { get; set; }
        public bool PrintPrefix { get; set; }
        public bool PrintSuffix { get; set; }
        public bool PrintInFront { get; set; }
        public string Comment { get; set; }
        public int Number { get; set; }
        public DateTime Created { get; set; }
    }
}
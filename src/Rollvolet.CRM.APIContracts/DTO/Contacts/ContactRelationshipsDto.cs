using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Contacts
{
    public class ContactRelationshipsDto
    {
        public IRelationship Country { get; set; }
        public IRelationship Language { get; set; }
        [JsonProperty("honorific-prefix")]
        public IRelationship HonorificPrefix { get; set; }
        public IRelationship Telephones { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Requests { get; set; }
        public IRelationship Invoices { get; set; }
    }
}
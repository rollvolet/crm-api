using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Telephones
{
    public class TelephoneAttributesDto
    {
        public string Area { get; set; }
        public string Number { get; set; }
        public string Memo { get; set; }
        public int Order { get; set; }
    }
}
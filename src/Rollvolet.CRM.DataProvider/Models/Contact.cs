using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Contact : CustomerRecord
    {        
        public Customer Customer { get; set; }
    }
}
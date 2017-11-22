using System.Collections.Generic;
using System.Linq;

namespace Rollvolet.CRM.Domain.Models.Query
{
    public class FilterQuery
    {
        public IDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    }
}
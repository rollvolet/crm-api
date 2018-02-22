using System.Collections.Generic;
using System.Linq;

namespace Rollvolet.CRM.Domain.Models.Query
{
    public class IncludeQuery
    {
        public string[] Fields { get; set; } = new string[0];
        
        public bool Contains(string field)
        {
            return Fields.ToList().Contains(field);
        }

        public IncludeQuery NestedInclude(string resourceName) {
            var fields = this.Fields.Where(x => x.StartsWith($"{resourceName}."))
                                            .Select(x => x.Substring($"{resourceName}.".Length)).ToArray();
            return new IncludeQuery {
                Fields = fields
            };
        }
    }
}
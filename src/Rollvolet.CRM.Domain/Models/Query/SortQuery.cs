using System.Collections.Generic;
using System.Linq;

namespace Rollvolet.CRM.Domain.Models.Query
{
    public class SortQuery
    {
        public static readonly string ORDER_ASC = "ASC";
        public static readonly string ORDER_DESC = "DESC";
        
        public string Field { get; set; }
        
        public string Order { get; set; } = ORDER_ASC;
    }
}
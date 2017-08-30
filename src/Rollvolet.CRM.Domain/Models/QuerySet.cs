namespace Rollvolet.CRM.Domain.Models
{
    public class QuerySet
    {
        public PageQuery Page { get; set; }

        public QuerySet()
        {
            Page = new PageQuery();
        }
    }
}
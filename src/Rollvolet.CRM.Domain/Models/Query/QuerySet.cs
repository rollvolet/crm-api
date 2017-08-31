namespace Rollvolet.CRM.Domain.Models.Query
{
    public class QuerySet
    {
        public PageQuery Page { get; set; } = new PageQuery();
        public IncludeQuery Include { get; set; } = new IncludeQuery();
    }
}
namespace Rollvolet.CRM.Domain.Models.Query
{
    public class QuerySet
    {
        public PageQuery Page { get; set; } = new PageQuery();
        public IncludeQuery Include { get; set; } = new IncludeQuery();
        public SortQuery Sort { get; set; } = new SortQuery();
        public FilterQuery Filter { get; set; } = new FilterQuery();
    }
}
namespace Rollvolet.CRM.Domain.Models.Interfaces
{
    public interface IPaged
    {
        int Count { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        bool HasPrev { get; }
        bool HasNext { get; }
        int First { get; }
        int Last { get; }
        int Prev { get; }
        int Next { get; }

    }
}
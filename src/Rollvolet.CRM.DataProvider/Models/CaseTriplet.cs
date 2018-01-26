namespace Rollvolet.CRM.DataProvider.Models
{
    public class CaseTriplet<T>
    {
        public T Source { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
    }
}
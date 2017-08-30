namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class CollectionLinks : Links
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
    }
}
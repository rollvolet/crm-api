namespace Rollvolet.CRM.APIContracts.JsonApi
{
    public class Error
    {
        public object Meta { get; set; }
        public object Links { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public object Source { get; set; }
    }
}
namespace Rollvolet.CRM.Domain.Models.Query
{
    public class PageQuery
    {
        public int Number { get; set; } = 0;
        public int Size { get; set; } = -1;

        public int Skip
        {
            get
            {
                return Number * Size;
            }
        }

        public int Take
        {
            get
            {
                return Size;
            }
        }

        public bool IsPaged
        {
            get
            {
                return Size >= 0;
            }
        }
    }
}
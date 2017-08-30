using System.Collections.Generic;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Paged<T> : IPaged
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool HasPrev
        {
            get
            {
                return PageNumber > 1;
            }
        }

        public bool HasNext
        {
            get
            {
                return PageNumber * PageSize < Count;
            }
        }

        public int First { get; } = 1;

        public int Last
        {
            get
            {
                return Count / PageSize;
            }
        }

        public int Next
        {
            get
            {
                return PageNumber + 1;
            }
        }

        public int Prev
        {
            get
            {
                return PageNumber - 1;
            }
        }
    }
}
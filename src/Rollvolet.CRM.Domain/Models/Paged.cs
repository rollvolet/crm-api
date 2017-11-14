using System;
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
                return PageNumber > 0;
            }
        }

        public bool HasNext
        {
            get
            {
                return (PageNumber + 1) * PageSize < Count;
            }
        }

        public int First { get; } = 0;

        public int Last
        {
            get
            {
                return PageSize < Count ? Convert.ToInt32(Math.Ceiling(Count / (PageSize * 1.0))) - 1 : First;
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
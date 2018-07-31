using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class OfferlineQueryExtensions
    {
        public static IQueryable<Offerline> Include(this IQueryable<Offerline> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Offerline, object>>>();

            selectors.Add("offer", c => c.Offer);
            selectors.Add("vat-rate", c => c.VatRate);

            return source.Include<Offerline>(querySet, selectors);
        }

        public static IQueryable<Offerline> Sort(this IQueryable<Offerline> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Offerline, object>>>();

            selectors.Add("amount", x => x.Amount);
            selectors.Add("sequence-number", x => x.SequenceNumber);

            return source.Sort<Offerline>(querySet, selectors);
        }
    }
}
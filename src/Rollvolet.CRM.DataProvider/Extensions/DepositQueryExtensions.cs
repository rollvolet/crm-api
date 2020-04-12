using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class DepositQueryExtensions
    {
        public static IQueryable<Deposit> Filter(this IQueryable<Deposit> source, QuerySet querySet)
        {
            return source;
        }

        public static IQueryable<Deposit> Include(this IQueryable<Deposit> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Deposit, object>>>();

            selectors.Add("payment", c => c.Payment);
            selectors.Add("customer", c => c.Customer);
            selectors.Add("order", c => c.Order);
            selectors.Add("invoice", c => c.Invoice);

            return source.Include<Deposit>(querySet, selectors);
        }

        public static IQueryable<Deposit> Sort(this IQueryable<Deposit> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Deposit, object>>>();

            selectors.Add("number", x => x.SequenceNumber);
            selectors.Add("payment-date", x => x.PaymentDate);
            selectors.Add("amount", x => x.Amount);
            selectors.Add("payment", x => x.Payment.Name);

            return source.Sort<Deposit>(querySet, selectors);
        }

    }
}
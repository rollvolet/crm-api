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
    public static class TelephoneQueryExtensions
    {
        public static IQueryable<Telephone> Include(this IQueryable<Telephone> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Telephone, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("telephone-type", c => c.TelephoneType);

            return source.Include<Telephone>(querySet, selectors);         
        }

        public static IQueryable<Telephone> Sort(this IQueryable<Telephone> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Telephone, object>>>();
            
            selectors.Add("order", x => x.Order.ToString());

            return source.Sort<Telephone>(querySet, selectors);
        }
    }
}
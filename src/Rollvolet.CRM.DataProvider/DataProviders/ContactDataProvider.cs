using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{   
    public class ContactDataProvider : IContactDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public ContactDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var skip = (query.Page.Number - 1) * query.Page.Size;
            var take = query.Page.Size;

            var source = _context.Contacts.Where(c => c.CustomerId == customerId);

            var sourceQuery = AddIncludeClauses(source, query);
            sourceQuery = AddSortClause(sourceQuery, query);

            var contacts = await sourceQuery.Skip(skip).Take(take).ToListAsync();
            var count = await source.CountAsync();

            var mappedContacts = _mapper.Map<IEnumerable<Contact>>(contacts);

            return new Paged<Contact>() {
                Items = mappedContacts,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        private IQueryable<DataProvider.Models.Contact> AddIncludeClauses(IQueryable<DataProvider.Models.Contact> sourceQuery, QuerySet query)
        {
            foreach (var field in query.Include.Fields)
            {
                if ("country".Equals(field))
                    sourceQuery = sourceQuery.Include(c => c.Country);
            }

            return sourceQuery;
        }

        private IQueryable<DataProvider.Models.Contact> AddSortClause(IQueryable<DataProvider.Models.Contact> sourceQuery, QuerySet query)
        {
            Expression<Func<DataProvider.Models.Contact, string>> selector = null;

            switch (query.Sort.Field)
            {
                case "name":
                    selector = x => x.Name;
                    break;
                default:
                    selector = null;
                    break;
            }

            if (selector != null)
            {
                sourceQuery = query.Sort.Order == SortQuery.ORDER_ASC ? sourceQuery.OrderBy(selector) : sourceQuery.OrderByDescending(selector);
            }

            return sourceQuery;
        }
    }
}
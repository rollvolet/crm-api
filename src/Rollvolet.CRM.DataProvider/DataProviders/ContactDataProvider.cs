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
using Rollvolet.CRM.DataProvider.Extensions;

namespace Rollvolet.CRM.DataProviders
{   
    public class ContactDataProvider : IContactDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ITelephoneDataProvider _telephoneDataProvider;

        public ContactDataProvider(CrmContext context, IMapper mapper, ITelephoneDataProvider telephoneDataProvider)
        {
            _context = context;
            _mapper = mapper;
            _telephoneDataProvider = telephoneDataProvider;
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            var contact = _context.Contacts.Single(x => x.DataId == id);

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Contacts
                            .Where(c => c.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var contacts = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();

            var count = await source.CountAsync();

            var mappedContacts = _mapper.Map<IEnumerable<Contact>>(contacts);

            return new Paged<Contact>() {
                Items = mappedContacts,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Rollvolet.CRM.DataProviders
{
    public class ContactDataProvider : CustomerRecordDataProvider, IContactDataProvider
    {
        public ContactDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider,
                                    ILogger<ContactDataProvider> logger) : base(context, mapper, sequenceDataProvider, logger)
        {
        }

        public async Task<Contact> GetByIdAsync(int id, QuerySet query = null)
        {
            var contact = await FindByIdAsync(id, query);

            if (contact == null)
            {
                _logger.LogError($"No contact found with data id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Contacts
                            .Where(c => c.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var contacts = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedContacts = _mapper.Map<IEnumerable<Contact>>(contacts);

            return new Paged<Contact>() {
                Items = mappedContacts,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Contact> GetByTelephoneIdAsync(string telephoneId)
        {
            var id = DataProvider.Models.Telephone.DecomposeCustomerRecordId(telephoneId);

            var contact = await FindByIdAsync(id);

            if (contact == null)
            {
                _logger.LogError($"No contact found with data id {id}, extracted from telephone id {telephoneId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> GetByRequestIdAsync(int requestId)
        {
            var request = await _context.Requests.Where(r => r.Id == requestId).FirstOrDefaultAsync();
            
            DataProvider.Models.Contact contact = null;
            if (request != null)
                contact = await _context.Contacts.Where(c => c.CustomerId == request.CustomerId && c.Number == request.RelativeContactId).FirstOrDefaultAsync();

            if (contact == null)
            {
                _logger.LogError($"No contact found for request id {requestId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            var contactRecord = _mapper.Map<DataProvider.Models.Contact>(contact);

            var customerId = contact.Customer.Id;
            contactRecord.Number = await _sequenceDataProvider.GetNextRelativeContactNumber(customerId);
            contactRecord.CustomerId = customerId;
            contactRecord.Created = DateTime.Now;
            contactRecord.SearchName = CalculateSearchName(contact.Name);

            await HydratePostalCode(contact, contactRecord);

            _context.Contacts.Add(contactRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Contact>(contactRecord);
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            var contactRecord = await FindByIdAsync(contact.Id);
            _mapper.Map(contact, contactRecord);
            contactRecord.SearchName = CalculateSearchName(contact.Name);

            await HydratePostalCode(contact, contactRecord);

            _context.Contacts.Update(contactRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Contact>(contactRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var contact = await FindByIdAsync(id);

            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Contact> FindByIdAsync(int id, QuerySet query = null)
        {
            var source = _context.Contacts.Where(c => c.DataId == id);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}
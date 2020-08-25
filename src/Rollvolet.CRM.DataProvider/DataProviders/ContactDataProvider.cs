using System;
using System.Collections.Generic;
using System.Linq;
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

            var contacts = await source.ForPage(query).ToListAsync();

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

        public async Task<Contact> GetByInterventionIdAsync(int interventionId)
        {
            var intervention = await _context.Interventions.Where(r => r.Id == interventionId).FirstOrDefaultAsync();

            DataProvider.Models.Contact contact = null;
            if (intervention != null)
                contact = await _context.Contacts.Where(c => c.CustomerId == intervention.CustomerId && c.Number == intervention.RelativeContactId).FirstOrDefaultAsync();

            if (contact == null)
            {
                _logger.LogError($"No contact found for intervention id {interventionId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> GetByOfferIdAsync(int offerId)
        {
            var offer = await _context.Offers.Where(r => r.Id == offerId).FirstOrDefaultAsync();

            DataProvider.Models.Contact contact = null;
            if (offer != null)
                contact = await _context.Contacts.Where(c => c.CustomerId == offer.CustomerId && c.Number == offer.RelativeContactId).FirstOrDefaultAsync();

            if (contact == null)
            {
                _logger.LogError($"No contact found for offer id {offerId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> GetByOrderIdAsync(int orderId)
        {
            var order = await _context.Orders.Where(r => r.Id == orderId).FirstOrDefaultAsync();

            DataProvider.Models.Contact contact = null;
            if (order != null)
                contact = await _context.Contacts.Where(c => c.CustomerId == order.CustomerId && c.Number == order.RelativeContactId).FirstOrDefaultAsync();

            if (contact == null)
            {
                _logger.LogError($"No contact found for order id {orderId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> GetByInvoiceIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.Where(r => r.Id == invoiceId).FirstOrDefaultAsync();

            DataProvider.Models.Contact contact = null;
            if (invoice != null)
                contact = await _context.Contacts.Where(c => c.CustomerId == invoice.CustomerId && c.Number == invoice.RelativeContactId).FirstOrDefaultAsync();

            if (contact == null)
            {
                _logger.LogError($"No contact found for invoice id {invoiceId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Contact>(contact);
        }

        public async Task<Contact> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await GetByInvoiceIdAsync(depositInvoiceId);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            var contactRecord = _mapper.Map<DataProvider.Models.Contact>(contact);

            var customerId = contact.Customer.Id;
            contactRecord.Number = await _sequenceDataProvider.GetNextRelativeContactNumberAsync(customerId);
            contactRecord.CustomerId = customerId;
            contactRecord.Created = DateTimeOffset.UtcNow.UtcDateTime;
            contactRecord.SearchName = DataProvider.Models.CustomerRecord.CalculateSearchName(contact.Name);

            await HydratePostalCodeAsync(contact, contactRecord);
            ReplaceEmptyStringWithNull(contact, contactRecord);

            _context.Contacts.Add(contactRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Contact>(contactRecord);
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            var contactRecord = await FindByIdAsync(contact.Id);
            _mapper.Map(contact, contactRecord);
            contactRecord.SearchName = DataProvider.Models.CustomerRecord.CalculateSearchName(contact.Name);

            await HydratePostalCodeAsync(contact, contactRecord);

            // Workaround constraints set on the database
            if (string.IsNullOrEmpty(contact.Suffix))
                contactRecord.Suffix = null;

            if (string.IsNullOrEmpty(contact.Prefix))
                contactRecord.Prefix = null;

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
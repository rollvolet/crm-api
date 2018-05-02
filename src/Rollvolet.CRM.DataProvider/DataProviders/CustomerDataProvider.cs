using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{   
    public class CustomerDataProvider : ICustomerDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ISequenceDataProvider _sequenceDataProvider;
        private readonly ITelephoneDataProvider _telephoneDataProvider;
        private readonly ILogger _logger;

        public CustomerDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider, 
                                    ITelephoneDataProvider telephoneDataProvider, ILogger<CustomerDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _sequenceDataProvider = sequenceDataProvider;
            _telephoneDataProvider = telephoneDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Customer>> GetAllAsync(QuerySet query)
        {
            var source = BaseQuery()
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var customers = source.ForPage(query).AsEnumerable();

            var mappedCustomers = _mapper.Map<IEnumerable<Customer>>(customers);

            var count = await source.CountAsync();

            return new Paged<Customer>() {
                Items = mappedCustomers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Customer> GetByNumberAsync(int number, QuerySet query = null)
        {
            var customer = await FindByNumberAsync(number, query);
            
            if (customer == null)
            {
                _logger.LogError($"No customer found with number {number}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Customer>(customer);
        }      

        public async Task<Customer> Create(Customer customer)
        {
            var customerRecord = _mapper.Map<DataProvider.Models.Customer>(customer);

            customerRecord.Number = await _sequenceDataProvider.GetNextCustomerNumber();
            customerRecord.Created = DateTime.Now;
            customerRecord.SearchName = CalculateSearchName(customer.Name);

            // The domain's customer.postalCode maps to the dataprovider's embeddedPostalCode property. 
            // We need to set the related postal code record manually here.
            if (customer.PostalCode != null)
            {
                var postalCodeRecord = await _context.PostalCodes.Where(p => p.Code == customer.PostalCode).FirstOrDefaultAsync();
                if (postalCodeRecord == null) {
                    _logger.LogDebug($"No PostalCode found with code '{customer.PostalCode}'");
                    throw new IllegalArgumentException("IllegalAttribute", $"Invalid postal code '{customer.PostalCode}'.");
                } else {
                    customerRecord.PostalCodeId = postalCodeRecord.Id;
                }
            }

            if (customer.Memo != null)
            {
                var memo = new DataProvider.Models.Memo() { Text = customer.Memo };
                customerRecord.Memo = memo;
                _context.Memos.Add(memo);
            }

            _context.Customers.Add(customerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Customer>(customerRecord);
        }

        public async Task DeleteByNumber(int number)
        {
            var customer = await FindByNumberAsync(number);

            if (customer != null)
            {
                if (customer.Memo != null)
                {
                    _context.Memos.Remove(customer.Memo);
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
           }
        }

        private IQueryable<DataProvider.Models.Customer> BaseQuery()
        {
            return _context.Customers
                        .Include(e => e.Memo);
        }

        public async Task<DataProvider.Models.Customer> FindByNumberAsync(int number, QuerySet query = null)
        {
            var source = BaseQuery()
                            .Where(c => c.Number == number);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }   

        private string CalculateSearchName(string name)
        {
            if (name != null)
            {
                var searchName = name.ToUpper();
                searchName = Regex.Replace(searchName, @"\s+", "");
                searchName = RemoveDiacritics(searchName);
                return searchName;                
            }
            
            return null;
        }

        // see https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        private string RemoveDiacritics(string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{
    public abstract class CustomerRecordDataProvider
    {
        protected readonly CrmContext _context;
        protected readonly IMapper _mapper;
        protected readonly ISequenceDataProvider _sequenceDataProvider;
        protected readonly ILogger _logger;

        public CustomerRecordDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider,
                                            ILogger<CustomerRecordDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _sequenceDataProvider = sequenceDataProvider;
            _logger = logger;
        }


        // The domain's contact.postalCode maps to the dataprovider's embeddedPostalCode property.
        // We need to set the related postal code record manually.
        protected async Task HydratePostalCodeAsync(CustomerEntity customerEntity, CustomerRecord customerRecord)
        {
            if (customerEntity.PostalCode != null)
            {
                var postalCodeRecord = await _context.PostalCodes.Where(p => p.Code == customerEntity.PostalCode).FirstOrDefaultAsync();
                if (postalCodeRecord == null) {
                    _logger.LogDebug($"No PostalCode found with code '{customerEntity.PostalCode}'. Probably a foreign postal code?");
                } else {
                    customerRecord.PostalCodeId = postalCodeRecord.Id;
                }
            }
        }

        // Workaround for constraints set on the database. Some fields don't allow an empty value. It must be set to null explicitly.
        protected void ReplaceEmptyStringWithNull(CustomerEntity customerEntity, CustomerRecord customerRecord)
        {
            if (string.IsNullOrEmpty(customerEntity.Suffix) || string.IsNullOrEmpty(customerEntity.Suffix.Trim()))
                customerRecord.Suffix = null;

            if (string.IsNullOrEmpty(customerEntity.Prefix) || string.IsNullOrEmpty(customerEntity.Prefix.Trim()))
                customerRecord.Prefix = null;

            if (string.IsNullOrEmpty(customerEntity.Email) || string.IsNullOrEmpty(customerEntity.Email.Trim()))
                customerRecord.Email = null;

            if (string.IsNullOrEmpty(customerEntity.Email2) || string.IsNullOrEmpty(customerEntity.Email2.Trim()))
                customerRecord.Email2 = null;

            if (string.IsNullOrEmpty(customerEntity.Comment) || string.IsNullOrEmpty(customerEntity.Comment.Trim()))
                customerRecord.Comment = null;
        }

        protected string CalculateSearchName(string name)
        {
            if (name != null)
            {
                var searchName = name.ToUpper();
                searchName = Regex.Replace(searchName, @"\s+", "");
                searchName = searchName.FilterDiacritics();
                return searchName;
            }

            return null;
        }
    }
}
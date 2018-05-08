using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
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
        protected async Task HydratePostalCode(CustomerEntity customerEntity, CustomerRecord customerRecord)
        {
            if (customerEntity.PostalCode != null)
            {
                var postalCodeRecord = await _context.PostalCodes.Where(p => p.Code == customerEntity.PostalCode).FirstOrDefaultAsync();
                if (postalCodeRecord == null) {
                    _logger.LogDebug($"No PostalCode found with code '{customerEntity.PostalCode}'");
                    throw new IllegalArgumentException("IllegalAttribute", $"Invalid postal code '{customerEntity.PostalCode}'.");
                } else {
                    customerRecord.PostalCodeId = postalCodeRecord.Id;
                }
            }
        }

        protected string CalculateSearchName(string name)
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
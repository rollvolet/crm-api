using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class SequenceDataProvider : ISequenceDataProvider
    {
        private readonly CrmContext _context;
        private int _customerNumber { get; set; } = 0;

        public SequenceDataProvider(CrmContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextCustomerNumber()
        {
            if (_customerNumber == 0)
            {
               _customerNumber =  _context.Customers.Max(c => c.Number);
            }

            _customerNumber++;

            return _customerNumber;
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;

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
               _customerNumber =  await _context.Customers.MaxAsync(c => c.Number);
            }

            _customerNumber++;

            return _customerNumber;
        }

        public async Task<int> GetNextRelativeContactNumber(int customerId)
        {
            var count = await _context.Contacts.Where(x => x.CustomerId == customerId).CountAsync();
            return count + 1;
        }

        public async Task<int> GetNextRelativeBuildingNumber(int customerId)
        {
            var count = await _context.Buildings.Where(x => x.CustomerId == customerId).CountAsync();
            return count + 1;
        }
    }
}
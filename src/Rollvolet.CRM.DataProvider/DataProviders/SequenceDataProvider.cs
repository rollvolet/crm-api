using System;
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

        public async Task<int> GetNextCustomerNumberAsync()
        {
            if (_customerNumber == 0)
            {
               _customerNumber =  await _context.Customers.MaxAsync(c => c.Number);
            }

            _customerNumber++;

            return _customerNumber;
        }

        public async Task<int> GetNextInvoiceNumberAsync()
        {
            // Not working with a local cached number because invoice numbers need to be sequential
            // TODO take year into account?
            var number = (int) await _context.Invoices.Where(i => i.Number != null).MaxAsync(c => c.Number);
            number++;
            return number;
        }

        public async Task<short> GetNextOfferSequenceNumberAsync(DateTime date)
        {
            var dateAtMidnight = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var tomorrowAtMidnight = dateAtMidnight.AddDays(1);

            var count = await _context.Offers.Where(x => x.OfferDate >= dateAtMidnight && x.OfferDate <= tomorrowAtMidnight).CountAsync();
            return (Int16) (count + 1);
        }

        public async Task<short> GetNextDepositSequenceNumberAsync(int orderId)
        {
            var count = await _context.Deposits.Where(x => x.OrderId == orderId).CountAsync();
            return (Int16) (count + 1);
        }

        public async Task<int> GetNextRelativeContactNumberAsync(int customerId)
        {
            var count = await _context.Contacts.Where(x => x.CustomerId == customerId).CountAsync();
            return count + 1;
        }

        public async Task<int> GetNextRelativeBuildingNumberAsync(int customerId)
        {
            var count = await _context.Buildings.Where(x => x.CustomerId == customerId).CountAsync();
            return count + 1;
        }
    }
}
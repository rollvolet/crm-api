using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class CaseDataProvider : ICaseDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CaseDataProvider(CrmContext context, IMapper mapper, ILogger<CaseDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }       

        public async Task<Case> GetCaseByRequestId(int requestId)
        {
            return await _context.Requests
                .Where(r => r.Id == requestId)
                .Include(r => r.Offer)
                    .ThenInclude(o => o.Order)
                        .ThenInclude(o => o.Invoice)
                .Select(x => new Case {
                    CustomerId = x.CustomerId,
                    RequestId = x.Id,
                    OfferId = x.Offer != null ? x.Offer.Id : (int?) null,
                    OrderId = x.Offer != null && x.Offer.Order != null ? x.Offer.Order.Id : (int?) null,
                    InvoiceId = x.Offer != null && x.Offer.Order != null && x.Offer.Order.Invoice != null ? x.Offer.Order.Invoice.Id : (int?) null  
                })
                .FirstOrDefaultAsync();
            // TODO: throw entitynotfound exception if case == null
        }

        public async Task<Case> GetCaseByOfferId(int offerId)
        {
            return await _context.Offers
                .Where(o => o.Id == offerId)
                .Include(o => o.Request)
                .Include(o => o.Order)
                    .ThenInclude(o => o.Invoice)
                .Select(x => new Case {
                    CustomerId = x.CustomerId,
                    RequestId = x.RequestId,
                    OfferId = x.Id,
                    OrderId = x.Order != null ? x.Order.Id : (int?) null,
                    InvoiceId = x.Order != null && x.Order.Invoice != null ? x.Order.Invoice.Id : (int?) null    
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Case> GetCaseByOrderId(int orderId)
        {
            return await _context.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.Offer)
                    .ThenInclude(o => o.Request)
                .Include(o => o.Invoice)
                .Select(x => new Case {
                    CustomerId = x.CustomerId,
                    RequestId = x.Offer != null && x.Offer.Request != null ? x.Offer.Request.Id : (int?) null,
                    OfferId = x.Offer != null ? x.Offer.Id : (int?) null,
                    OrderId = x.Id,
                    InvoiceId = x.Invoice != null ? x.Invoice.Id : (int?) null                   
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Case> GetCaseByInvoiceId(int invoiceId)
        {
            return await _context.Invoices
                .Where(x => x.Id == invoiceId)
                .Include(x => x.Order)
                    .ThenInclude(o => o.Offer)
                        .ThenInclude(o => o.Request)
                .Select(x => new Case {
                    CustomerId = x.CustomerId,
                    RequestId = x.Order != null && x.Order.Offer != null && x.Order.Offer.Request != null ? x.Order.Offer.Request.Id : (int?) null,
                    OfferId = x.Order != null && x.Order.Offer != null ? x.Order.Offer.Id : (int?) null,
                    OrderId = x.Order != null ? x.Order.Id : (int?) null,
                    InvoiceId = x.Id               
                })
                .FirstOrDefaultAsync();
        }
    }
}
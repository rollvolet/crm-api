using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.DataProvider.Models.Interfaces;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
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

        public async Task<Case> GetCaseByRequestIdAsync(int requestId)
        {
            var source = _context.Requests
                .Where(r => r.Id == requestId)
                .Include(r => r.Customer)
                .Include(r => r.Building)
                .Include(r => r.Contact)
                .Include(r => r.Offer)
                    .ThenInclude(o => o.Order)
                        .ThenInclude(o => o.Invoice);

            var result = await source.Select(x => new Case {
                    CustomerId = x.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Id,
                    OfferId = x.Offer != null ? x.Offer.Id : (int?) null,
                    OrderId = x.Offer != null && x.Offer.Order != null ? x.Offer.Order.Id : (int?) null,
                    InvoiceId = x.Offer != null && x.Offer.Order != null && x.Offer.Order.Invoice != null ? x.Offer.Order.Invoice.Id : (int?) null
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with requestId {requestId}");
                throw new EntityNotFoundException();
            }

            return result;
        }

        public async Task<Case> GetCaseByInterventionIdAsync(int interventionId)
        {
            var source = _context.Interventions
                .Where(r => r.Id == interventionId)
                .Include(r => r.Customer)
                .Include(r => r.Building)
                .Include(r => r.Contact)
                .Include(r => r.Invoice);

            var result = await source.Select(x => new Case {
                    CustomerId = x.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    InterventionId = x.Id,
                    InvoiceId = x.Invoice != null ? x.Invoice.Id : (int?) null
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with interventionId {interventionId}");
                throw new EntityNotFoundException();
            }

            return result;
        }

        public async Task<Case> GetCaseByOfferIdAsync(int offerId)
        {
            var source = _context.Offers
                .Where(o => o.Id == offerId)
                .Include(r => r.Customer)
                .Include(r => r.Building)
                .Include(r => r.Contact)
                .Include(o => o.Request)
                .Include(o => o.Order)
                    .ThenInclude(o => o.Invoice);

            var result = await source.Select(x => new Case {
                    CustomerId = x.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.RequestId,
                    OfferId = x.Id,
                    OrderId = x.Order != null ? x.Order.Id : (int?) null,
                    InvoiceId = x.Order != null && x.Order.Invoice != null ? x.Order.Invoice.Id : (int?) null
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with offerId {offerId}");
                throw new EntityNotFoundException();
            }

            return result;
        }

        public async Task<Case> GetCaseByOrderIdAsync(int orderId)
        {
            var source = _context.Orders
                .Where(o => o.Id == orderId)
                .Include(r => r.Customer)
                .Include(r => r.Building)
                .Include(r => r.Contact)
                .Include(o => o.Offer)
                    .ThenInclude(o => o.Request)
                .Include(o => o.Invoice);

            var result = await source.Select(x => new Case {
                    CustomerId = x.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Offer != null && x.Offer.Request != null ? x.Offer.Request.Id : (int?) null,
                    OfferId = x.Offer != null ? x.Offer.Id : (int?) null,
                    OrderId = x.Id,
                    InvoiceId = x.Invoice != null ? x.Invoice.Id : (int?) null
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with orderId {orderId}");
                throw new EntityNotFoundException();
            }

            return result;
        }

        public async Task<Case> GetCaseByInvoiceIdAsync(int invoiceId)
        {
            var source = _context.Invoices
                .Where(x => x.Id == invoiceId)
                .Include(r => r.Customer)
                .Include(r => r.Building)
                .Include(r => r.Contact)
                .Include(x => x.Order)
                    .ThenInclude(o => o.Offer)
                        .ThenInclude(o => o.Request)
                .Include(x => x.Intervention);

            var result = await source.Select(x => new Case {
                    CustomerId = x.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Order != null && x.Order.Offer != null && x.Order.Offer.Request != null ? x.Order.Offer.Request.Id : (int?) null,
                    OfferId = x.Order != null && x.Order.Offer != null ? x.Order.Offer.Id : (int?) null,
                    OrderId = x.Order != null ? x.Order.Id : (int?) null,
                    InterventionId = x.Intervention != null ? x.Intervention.Id : (int?) null,
                    InvoiceId = x.Id
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with invoiceId {invoiceId}");
                throw new EntityNotFoundException();
            }

            return result;
        }
    }
}
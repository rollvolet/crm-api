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
                .Include(r => r.Offer)
                    .ThenInclude(o => o.Order)
                        .ThenInclude(o => o.Invoice);

            var joinedSource = JoinBuildingAndContact(source);

            var result = await joinedSource.Select(x => new Case {
                    CustomerId = x.Source.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Source.Id,
                    OfferId = x.Source.Offer != null ? x.Source.Offer.Id : (int?) null,
                    OrderId = x.Source.Offer != null && x.Source.Offer.Order != null ? x.Source.Offer.Order.Id : (int?) null,
                    InvoiceId = x.Source.Offer != null && x.Source.Offer.Order != null && x.Source.Offer.Order.Invoice != null ? x.Source.Offer.Order.Invoice.Id : (int?) null
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
                .Include(r => r.Invoice);

            var joinedSource = JoinBuildingAndContact(source);

            var result = await joinedSource.Select(x => new Case {
                    CustomerId = x.Source.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    InterventionId = x.Source.Id,
                    InvoiceId = x.Source.Invoice != null ? x.Source.Invoice.Id : (int?) null
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
                .Include(o => o.Request)
                .Include(o => o.Order)
                    .ThenInclude(o => o.Invoice);

            var joinedSource = JoinBuildingAndContact(source);

            var result = await joinedSource.Select(x => new Case {
                    CustomerId = x.Source.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Source.RequestId,
                    OfferId = x.Source.Id,
                    OrderId = x.Source.Order != null ? x.Source.Order.Id : (int?) null,
                    InvoiceId = x.Source.Order != null && x.Source.Order.Invoice != null ? x.Source.Order.Invoice.Id : (int?) null
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
                .Include(o => o.Offer)
                    .ThenInclude(o => o.Request)
                .Include(o => o.Invoice);

            var joinedSource = JoinBuildingAndContact(source);

            var result = await joinedSource.Select(x => new Case {
                    CustomerId = x.Source.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Source.Offer != null && x.Source.Offer.Request != null ? x.Source.Offer.Request.Id : (int?) null,
                    OfferId = x.Source.Offer != null ? x.Source.Offer.Id : (int?) null,
                    OrderId = x.Source.Id,
                    InvoiceId = x.Source.Invoice != null ? x.Source.Invoice.Id : (int?) null
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
                .Include(x => x.Order)
                    .ThenInclude(o => o.Offer)
                        .ThenInclude(o => o.Request)
                .Include(x => x.Intervention);

            var joinedSource = JoinBuildingAndContact(source);

            var result = await joinedSource.Select(x => new Case {
                    CustomerId = x.Source.CustomerId,
                    ContactId = x.Contact != null ? x.Contact.DataId : (int?) null,
                    BuildingId = x.Building != null ? x.Building.DataId : (int?) null,
                    RequestId = x.Source.Order != null && x.Source.Order.Offer != null && x.Source.Order.Offer.Request != null ? x.Source.Order.Offer.Request.Id : (int?) null,
                    OfferId = x.Source.Order != null && x.Source.Order.Offer != null ? x.Source.Order.Offer.Id : (int?) null,
                    OrderId = x.Source.Order != null ? x.Source.Order.Id : (int?) null,
                    InterventionId = x.Source.Intervention != null ? x.Source.Intervention.Id : (int?) null,
                    InvoiceId = x.Source.Id
                })
                .FirstOrDefaultAsync();

            if (result == null) {
                _logger.LogError($"No case found with invoiceId {invoiceId}");
                throw new EntityNotFoundException();
            }

            return result;
        }

        // TODO this method is duplicated in CaseRelatedDataProvider
        private IQueryable<CaseTriplet<T>> JoinBuildingAndContact<T>(IQueryable<T> source) where T : ICaseRelated
        {
            return source.GroupJoin(
                    _context.Buildings.Include(b => b.HonorificPrefix),
                    s => new { CustomerId = s.CustomerId, Number = s.RelativeBuildingId },
                    b => new { CustomerId = (int?) b.CustomerId, Number = (int?) b.Number },
                    (s, b) => new { Source = s, Buildings = b }
                ).SelectMany(
                    t => t.Buildings.DefaultIfEmpty(),
                    (t, b) => new { Source = t.Source, Building = b }
                ).GroupJoin(
                    _context.Contacts.Include(c => c.HonorificPrefix),
                    t => new { CustomerId = t.Source.CustomerId, Number = t.Source.RelativeContactId },
                    c => new { CustomerId = (int?) c.CustomerId, Number = (int?) c.Number },
                    (t, c) => new { Source = t.Source, Building = t.Building, Contacts = c }
                ).SelectMany(
                    u => u.Contacts.DefaultIfEmpty(),
                    (u, c) => new CaseTriplet<T> { Source = u.Source, Building = u.Building, Contact = c}
                );
        }
    }
}
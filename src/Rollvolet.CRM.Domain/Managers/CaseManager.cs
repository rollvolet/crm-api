using System.Linq;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class CaseManager : ICaseManager
    {
        private readonly ICaseDataProvider _caseDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IRequestManager _requestManager;
        private readonly IOrderManager _orderManager;

        public CaseManager(ICaseDataProvider caseDataProvider, IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                            IDepositInvoiceDataProvider depositInvoiceDataProvider, IRequestDataProvider requestDataProvider,
                            IOfferDataProvider offerDataProvider, IInvoiceDataProvider invoiceDataProvider,
                            IRequestManager requestManager, IOrderManager orderManager)
        {
            _caseDataProvider = caseDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _requestDataProvider = requestDataProvider;
            _offerDataProvider = offerDataProvider;
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _requestManager = requestManager;
            _orderManager = orderManager;
        }

        public async Task<Case> GetCaseAsync(int? requestId, int? offerId, int? orderId, int? invoiceId)
        {
            var paramCount = new int?[] { requestId, offerId, orderId, invoiceId }.Where(p => p != null).Count();
            if (paramCount != 1)
              throw new IllegalArgumentException("InvalidCaseParams", $"Exactly 1 of requestId, offerId, orderId or invoiceId must be set. Found {paramCount} params.");
            if (requestId != null)
              return await _caseDataProvider.GetCaseByRequestIdAsync((int) requestId);
            else if (offerId != null)
              return await _caseDataProvider.GetCaseByOfferIdAsync((int) offerId);
            else if (orderId != null)
              return await _caseDataProvider.GetCaseByOrderIdAsync((int) orderId);
            else if (invoiceId != null)
              return await _caseDataProvider.GetCaseByInvoiceIdAsync((int) invoiceId);
            else
              return null;
        }

        // Note: contact and building of a Case can only be updated through this method
        // UpdateAsync() methods of a resource in OfferManager, OrdereManager, etc. prevent the update of the contact/building for an existing resource
        // That's why we directly call methods of the OfferDataProvider, OrderDataProvider, etc. here
        public async Task UpdateContactAndBuildingAsync(int? contactId, int? buildingId, int? requestId, int? offerId, int? orderId, int? invoiceId)
        {
            int? relativeContactId = null;
            int? relativeBuildingId = null;
            try
            {
                if (contactId != null)
                {
                    var contact = await _contactDataProvider.GetByIdAsync((int) contactId);
                    relativeContactId = contact.Number;
                }

                if (buildingId != null)
                {
                    var building = await _buildingDataProvider.GetByIdAsync((int) buildingId);
                    relativeBuildingId = building.Number;
                }

                if (requestId != null)
                {
                    await _requestDataProvider.UpdateContactAndBuildingAsync((int) requestId, relativeContactId, relativeBuildingId);
                    await _requestManager.SyncCalendarEventAsync((int) requestId);
                }

                if (offerId != null)
                    await _offerDataProvider.UpdateContactAndBuildingAsync((int) offerId, relativeContactId, relativeBuildingId);
                if (orderId != null)
                {
                    // updating the offer automatically updates the contact/building of the order too. No need to do that explicitly here.

                    await _orderManager.SyncPlanningEventAsync((int) orderId, true);

                    var query = new QuerySet();
                    query.Page.Size = 1000; // TODO we assume 1 case doesn't have more than 1000 deposit invoices. Ideally, we should query by page.
                    var depositInvoices = await _depositInvoiceDataProvider.GetAllByOrderIdAsync((int) orderId, query);

                    foreach (var depositInvoice in depositInvoices.Items)
                    {
                        await _depositInvoiceDataProvider.UpdateContactAndBuildingAsync(depositInvoice.Id, relativeContactId, relativeBuildingId);
                    }
                }
                if (invoiceId != null)
                    await _invoiceDataProvider.UpdateContactAndBuildingAsync((int) invoiceId, relativeContactId, relativeBuildingId);
            }
            catch (EntityNotFoundException e)
            {
                throw new IllegalArgumentException("IllegalEntity", $"Contact and building cannot be updated: ${e.Message}", e);
            }
        }
    }
}
using System;
using System.Collections.Generic;
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

        public CaseManager(ICaseDataProvider caseDataProvider)
        {
            _caseDataProvider = caseDataProvider;
        }

        public async Task<Case> GetCase(int? requestId, int? offerId, int? orderId, int? invoiceId)
        {
            if (requestId != null)
              return await _caseDataProvider.GetCaseByRequestIdAsync((int) requestId);
            else if (offerId != null)
              return await _caseDataProvider.GetCaseByOfferIdAsync((int) offerId);
            else if (orderId != null)
              return await _caseDataProvider.GetCaseByOrderIdAsync((int) orderId);
            else if (invoiceId != null)
              return await _caseDataProvider.GetCaseByInvoiceIdAsync((int) invoiceId);
            else
              throw new IllegalArgumentException("CaseParamMissing", "Either requestId, offerId, orderId or invoiceId must be set");
        }
    }
}
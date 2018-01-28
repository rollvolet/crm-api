using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
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
              return await _caseDataProvider.GetCaseByRequestId((int) requestId);
            else if (offerId != null)
              return await _caseDataProvider.GetCaseByOfferId((int) offerId);
            else
              throw new ArgumentException("Either requestId, offerId, orderId or invoiceId must be set");
        }
    }
}
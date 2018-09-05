using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class OfferlineManager : IOfferlineManager
    {
        private readonly IOfferlineDataProvider _offerlineDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly ILogger _logger;

        public OfferlineManager(IOfferlineDataProvider offerlineDataProvider, IOfferDataProvider offerDataProvider,
                                IVatRateDataProvider vatRateDataProvider, ILogger<OfferManager> logger)
        {
            _offerlineDataProvider = offerlineDataProvider;
            _offerDataProvider = offerDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _logger = logger;
        }

        public async Task<Offerline> GetByIdAsync(int id, QuerySet query)
        {
            return await _offerlineDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Offerline>> GetAllByOfferIdAsync(int offerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_ASC;
                query.Sort.Field = "sequence-number";
            }

            return await _offerlineDataProvider.GetAllByOfferIdAsync(offerId, query);
        }

        public async Task<Offerline> CreateAsync(Offerline offerline)
        {
            if (offerline.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Offerline cannot have an id on create.");
            if (offerline.Offer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer is required on offerline creation.");
            if (offerline.Amount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Amount is required on offerline creation.");

            await EmbedRelations(offerline);

            if (offerline.Offer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer is required.");
            if (offerline.VatRate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Vat-rate is required.");

            return await _offerlineDataProvider.CreateAsync(offerline);
        }

        public async Task<Offerline> UpdateAsync(Offerline offerline)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "offer", "vat-rate" };
            var existingOfferline = await _offerlineDataProvider.GetByIdAsync(offerline.Id, query);

            if (offerline.Id != existingOfferline.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Offerline id cannot be updated.");
            if (offerline.Amount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Amount is required on offerline.");

            await EmbedRelations(offerline, existingOfferline);

            if (offerline.Offer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer is required.");
            if (offerline.VatRate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Vat-rate is required.");

            return await _offerlineDataProvider.UpdateAsync(offerline);
        }

        public async Task DeleteAsync(int id)
        {
            await _offerlineDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in request resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Offerline offerline, Offerline oldOfferline = null)
        {
            try {
                if (offerline.VatRate != null)
                {
                    if (oldOfferline != null && oldOfferline.VatRate != null && oldOfferline.VatRate.Id == offerline.VatRate.Id)
                        offerline.VatRate = oldOfferline.VatRate;
                    else
                        offerline.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(offerline.VatRate.Id));
                }

                // Offer cannot be updated. Take Offer of oldOfferline on update.
                if (oldOfferline != null)
                    offerline.Offer = oldOfferline.Offer;
                else
                    offerline.Offer = await _offerDataProvider.GetByIdAsync(offerline.Offer.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}
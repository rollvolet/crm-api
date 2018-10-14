using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class BuildingDataProvider : CustomerRecordDataProvider, IBuildingDataProvider
    {
        public BuildingDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider,
                                    ILogger<BuildingDataProvider> logger) : base(context, mapper, sequenceDataProvider, logger)
        {
        }

        public async Task<Building> GetByIdAsync(int id, QuerySet query = null)
        {
            var building = await FindByIdAsync(id, query);

            if (building == null)
            {
                _logger.LogError($"No building found with data id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Buildings
                            .Where(c => c.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var buildings = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedBuildings = _mapper.Map<IEnumerable<Building>>(buildings);

            return new Paged<Building>() {
                Items = mappedBuildings,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Building> GetByTelephoneIdAsync(string telephoneId)
        {
            var id = DataProvider.Models.Telephone.DecomposeCustomerRecordId(telephoneId);

            var building = await FindByIdAsync(id);

            if (building == null)
            {
                _logger.LogError($"No building found with data id {id}, extracted from telephone id {telephoneId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Building> GetByRequestIdAsync(int requestId)
        {
            var request = await _context.Requests.Where(r => r.Id == requestId).FirstOrDefaultAsync();

            DataProvider.Models.Building building = null;
            if (request != null)
                building = await _context.Buildings.Where(c => c.CustomerId == request.CustomerId && c.Number == request.RelativeBuildingId).FirstOrDefaultAsync();

            if (building == null)
            {
                _logger.LogError($"No building found for request id {requestId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Building> GetByOfferIdAsync(int offerId)
        {
            var offer = await _context.Offers.Where(r => r.Id == offerId).FirstOrDefaultAsync();

            DataProvider.Models.Building building = null;
            if (offer != null)
                building = await _context.Buildings.Where(c => c.CustomerId == offer.CustomerId && c.Number == offer.RelativeBuildingId).FirstOrDefaultAsync();

            if (building == null)
            {
                _logger.LogError($"No building found for offer id {offerId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Building> GetByOrderIdAsync(int orderId)
        {
            var order = await _context.Orders.Where(r => r.Id == orderId).FirstOrDefaultAsync();

            DataProvider.Models.Building building = null;
            if (order != null)
                building = await _context.Buildings.Where(c => c.CustomerId == order.CustomerId && c.Number == order.RelativeBuildingId).FirstOrDefaultAsync();

            if (building == null)
            {
                _logger.LogError($"No building found for order id {orderId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Building> GetByInvoiceIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.Where(r => r.Id == invoiceId).FirstOrDefaultAsync();

            DataProvider.Models.Building building = null;
            if (invoice != null)
                building = await _context.Buildings.Where(c => c.CustomerId == invoice.CustomerId && c.Number == invoice.RelativeBuildingId).FirstOrDefaultAsync();

            if (building == null)
            {
                _logger.LogError($"No building found for invoice id {invoiceId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
        }

        public async Task<Building> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await GetByInvoiceIdAsync(depositInvoiceId);
        }

        public async Task<Building> CreateAsync(Building building)
        {
            var buildingRecord = _mapper.Map<DataProvider.Models.Building>(building);

            var customerId = building.Customer.Id;
            buildingRecord.Number = await _sequenceDataProvider.GetNextRelativeBuildingNumber(customerId);
            buildingRecord.CustomerId = customerId;
            buildingRecord.Created = DateTime.Now;
            buildingRecord.SearchName = CalculateSearchName(building.Name);

            await HydratePostalCode(building, buildingRecord);

            _context.Buildings.Add(buildingRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Building>(buildingRecord);
        }

        public async Task<Building> UpdateAsync(Building building)
        {
            var buildingRecord = await FindByIdAsync(building.Id);
            _mapper.Map(building, buildingRecord);
            buildingRecord.SearchName = CalculateSearchName(building.Name);

            await HydratePostalCode(building, buildingRecord);

            // Workaround constraints set on the database
            if (string.IsNullOrEmpty(building.Suffix))
                buildingRecord.Suffix = null;

            if (string.IsNullOrEmpty(building.Prefix))
                buildingRecord.Prefix = null;

            _context.Buildings.Update(buildingRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Building>(buildingRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var building = await FindByIdAsync(id);

            if (building != null)
            {
                _context.Buildings.Remove(building);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Building> FindByIdAsync(int id, QuerySet query = null)
        {
            var source = _context.Buildings.Where(c => c.DataId == id);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}
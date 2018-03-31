using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;

namespace Rollvolet.CRM.DataProviders
{   
    public class WorkingHourDataProvider : IWorkingHourDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public WorkingHourDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.WorkingHours
                            .Where(c => c.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var workingHours = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedWorkingHours = _mapper.Map<IEnumerable<WorkingHour>>(workingHours);

            return new Paged<WorkingHour>() {
                Items = mappedWorkingHours,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }
    }
}
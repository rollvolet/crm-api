using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Invoice : BaseInvoice
    {
        public bool IsCreditNote { get; set; }
        public bool HasProductionTicket { get; set; }
        
        public IEnumerable<InvoiceSupplement> Supplements { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
        public IEnumerable<WorkingHour> WorkingHours { get; set; }
    }
}
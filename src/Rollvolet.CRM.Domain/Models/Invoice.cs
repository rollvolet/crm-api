using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Invoice : BaseInvoice
    {
        public bool HasProductionTicket { get; set; }

        public Intervention Intervention { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
    }
}
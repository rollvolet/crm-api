using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Invoice : BaseInvoice
    {
        public IEnumerable<InvoiceSupplement> Supplements { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
    }
}
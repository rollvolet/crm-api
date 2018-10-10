using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class DepositInvoice : BaseInvoice
    {
        public double? Amount { get; set; }
        public double? Vat { get; set; }
        public double? TotalAmount { get; set; } 
    }
}
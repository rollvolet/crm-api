using System;

namespace Rollvolet.CRM.APIContracts.DTO.Deposits
{
    public class DepositAttributesDto
    {
        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
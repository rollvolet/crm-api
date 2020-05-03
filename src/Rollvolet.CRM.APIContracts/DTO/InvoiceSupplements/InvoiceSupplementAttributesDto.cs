using System;

namespace Rollvolet.CRM.APIContracts.DTO.InvoiceSupplements
{
    public class InvoiceSupplementAttributesDto
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public double? NbOfPieces { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}

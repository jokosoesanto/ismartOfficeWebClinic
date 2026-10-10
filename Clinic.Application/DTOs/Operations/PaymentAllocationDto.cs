using System;
using System.Collections.Generic;

namespace Clinic.Application.DTOs.Operations
{
    public class PaymentAllocationDetailsDto
    {
        public Guid PaymentId { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal RemainingToAllocate => PaymentAmount - TotalAllocated;
        
        public List<InvoiceLineAllocationDto> InvoiceLines { get; set; } = new();
    }

    public class InvoiceLineAllocationDto
    {
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; } = null!;
        public decimal LineTotal { get; set; }
        public decimal AlreadyAllocated { get; set; }
        public decimal RemainingToAllocate => LineTotal - AlreadyAllocated;
        public decimal CurrentAllocation { get; set; } // Used for binding from form
    }
}

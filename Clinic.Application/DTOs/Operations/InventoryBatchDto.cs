using System;

namespace Clinic.Application.DTOs.Operations
{
    public class InventoryBatchDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string BatchNumber { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        
        // Computed properties
        public decimal CurrentStock { get; set; }
        public string Status { get; set; } = "Active";
    }
}

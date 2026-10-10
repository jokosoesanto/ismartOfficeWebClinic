using System;

namespace Clinic.Application.DTOs.Operations
{
    public class InventoryExpiryMonitoringDto
    {
        public Guid BatchId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string BatchNumber { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public int? DaysToExpiry { get; set; }
        public decimal CurrentStock { get; set; }
        public string ExpiryStatus { get; set; } = null!;
    }
}

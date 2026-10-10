using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Operations
{
    public class InventoryBatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }
        
        public string BatchNumber { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<InventoryStockTransaction> StockTransactions { get; set; } = new List<InventoryStockTransaction>();
    }
}

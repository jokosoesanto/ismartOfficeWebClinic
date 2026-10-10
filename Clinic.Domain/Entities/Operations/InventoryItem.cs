using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities.Operations
{
    public class InventoryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string Unit { get; set; } = null!;
        public decimal ReorderLevel { get; set; } = 0m;
        
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<InventoryStockTransaction> StockTransactions { get; set; } = new List<InventoryStockTransaction>();
        public ICollection<InventoryBatch> InventoryBatches { get; set; } = new List<InventoryBatch>();
    }
}

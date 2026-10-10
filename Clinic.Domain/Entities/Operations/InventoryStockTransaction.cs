using System;

namespace Clinic.Domain.Entities.Operations
{
    public class InventoryStockTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }
        
        public Guid? InventoryBatchId { get; set; }
        public InventoryBatch? InventoryBatch { get; set; }
        
        public Guid? SupplierId { get; set; }
        public Clinic.Domain.Entities.MasterData.Supplier? Supplier { get; set; }
        
        public DateTime TransactionDate { get; set; }
        public InventoryTransactionType TransactionType { get; set; }
        public decimal Quantity { get; set; }
        
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
    }
}

using System;

namespace Clinic.Application.DTOs.Operations
{
    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string Unit { get; set; } = null!;
        public decimal ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        
        // Calculated field for UI
        public decimal CurrentStock { get; set; }
        public string? StockStatus { get; set; }

        public System.Collections.Generic.List<InventoryStockTransactionDto> History { get; set; } = new System.Collections.Generic.List<InventoryStockTransactionDto>();
        public System.Collections.Generic.List<InventoryBatchDto> Batches { get; set; } = new System.Collections.Generic.List<InventoryBatchDto>();
    }
}

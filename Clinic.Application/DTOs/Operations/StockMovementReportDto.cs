using System;
using System.Collections.Generic;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.DTOs.Operations
{
    public class StockMovementFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public InventoryTransactionType? TransactionType { get; set; }
        public Guid? InventoryItemId { get; set; }
    }

    public class StockMovementSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal TotalStockIn { get; set; }
        public decimal TotalStockOut { get; set; }
        public decimal TotalAdjustmentIncrease { get; set; }
        public decimal TotalAdjustmentDecrease { get; set; }
    }

    public class StockMovementItemStatusDto
    {
        public Guid ItemId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal CurrentStock { get; set; }
        public string Status { get; set; } = null!;
    }

    public class StockMovementTransactionDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCode { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class StockMovementReportDto
    {
        public StockMovementFilterDto Filter { get; set; } = new();
        public StockMovementSummaryDto Summary { get; set; } = new();
        public List<StockMovementItemStatusDto> ItemStatuses { get; set; } = new();
        public List<StockMovementTransactionDto> Transactions { get; set; } = new();
        
        // For dropdown options
        public List<InventoryItemDto> AvailableItems { get; set; } = new();
    }
}

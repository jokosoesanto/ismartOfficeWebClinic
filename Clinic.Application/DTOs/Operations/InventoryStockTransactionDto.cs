using System;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.DTOs.Operations
{
    public class InventoryStockTransactionDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public Guid? InventoryBatchId { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionTypeName { get; set; } = null!;
        public InventoryTransactionType TransactionType { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal Quantity { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}

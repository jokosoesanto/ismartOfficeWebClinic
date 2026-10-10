using System;
using System.ComponentModel.DataAnnotations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.DTOs.Operations
{
    public class StockOperationDto
    {
        public Guid InventoryItemId { get; set; }
        
        public Guid? InventoryBatchId { get; set; }
        
        public Guid? SupplierId { get; set; }
        
        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        
        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public InventoryTransactionType TransactionType { get; set; }
        
        public string? Notes { get; set; }
    }
}

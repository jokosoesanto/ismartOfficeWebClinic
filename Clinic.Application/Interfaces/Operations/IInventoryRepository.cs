using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.DTOs.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.Interfaces.Operations
{
    public interface IInventoryRepository
    {
        Task<List<InventoryItemDto>> GetAllItemsAsync();
        Task<InventoryItem?> GetItemByIdAsync(Guid id);
        Task<InventoryItemDto?> GetItemDtoByIdAsync(Guid id);
        Task<InventoryItem> CreateItemAsync(InventoryItem item);
        Task UpdateItemAsync(InventoryItem item);
        
        Task<InventoryBatch> CreateBatchAsync(InventoryBatch batch);
        
        Task<InventoryStockTransaction> AddStockTransactionAsync(InventoryStockTransaction transaction);
        Task<StockMovementReportDto> GetStockMovementReportAsync(StockMovementFilterDto filter);
        Task<List<InventoryExpiryMonitoringDto>> GetExpiryMonitoringAsync(int nearExpiryThresholdDays);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories.Operations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItemDto>> GetAllItemsAsync()
        {
            var items = await _context.InventoryItems
                .Where(i => !i.IsDeleted)
                .Select(i => new
                {
                    Item = i,
                    StockLedger = _context.InventoryStockTransactions
                        .Include(t => t.Supplier)
                        .Where(t => t.InventoryItemId == i.Id)
                        .ToList(),
                    Batches = _context.InventoryBatches
                        .Where(b => b.InventoryItemId == i.Id && !b.IsDeleted)
                        .ToList()
                })
                .ToListAsync();

            var result = new List<InventoryItemDto>();

            foreach (var record in items)
            {
                var ledger = record.StockLedger;
                decimal currentStock = 0;
                
                foreach (var tx in ledger)
                {
                    if (tx.TransactionType == InventoryTransactionType.OpeningBalance ||
                        tx.TransactionType == InventoryTransactionType.StockIn ||
                        tx.TransactionType == InventoryTransactionType.AdjustmentIncrease)
                    {
                        currentStock += tx.Quantity;
                    }
                    else if (tx.TransactionType == InventoryTransactionType.StockOut ||
                             tx.TransactionType == InventoryTransactionType.AdjustmentDecrease)
                    {
                        currentStock -= tx.Quantity;
                    }
                }

                var batchDtos = new List<InventoryBatchDto>();
                foreach (var batch in record.Batches)
                {
                    decimal batchStock = 0;
                    foreach(var tx in ledger.Where(t => t.InventoryBatchId == batch.Id)) 
                    {
                        if (tx.TransactionType == InventoryTransactionType.OpeningBalance ||
                            tx.TransactionType == InventoryTransactionType.StockIn ||
                            tx.TransactionType == InventoryTransactionType.AdjustmentIncrease)
                        {
                            batchStock += tx.Quantity;
                        }
                        else if (tx.TransactionType == InventoryTransactionType.StockOut ||
                                 tx.TransactionType == InventoryTransactionType.AdjustmentDecrease)
                        {
                            batchStock -= tx.Quantity;
                        }
                    }
                    
                    string status = "Active";
                    if (!batch.ExpiryDate.HasValue) status = "No Expiry";
                    else if (batch.ExpiryDate.Value.Date < DateTime.UtcNow.Date) status = "Expired";

                    batchDtos.Add(new InventoryBatchDto
                    {
                        Id = batch.Id,
                        InventoryItemId = batch.InventoryItemId,
                        BatchNumber = batch.BatchNumber,
                        ExpiryDate = batch.ExpiryDate,
                        IsActive = batch.IsActive,
                        CurrentStock = batchStock,
                        Status = status
                    });
                }

                result.Add(new InventoryItemDto
                {
                    Id = record.Item.Id,
                    Code = record.Item.Code,
                    Name = record.Item.Name,
                    Category = record.Item.Category,
                    Unit = record.Item.Unit,
                    IsActive = record.Item.IsActive,
                    ReorderLevel = record.Item.ReorderLevel,
                    CurrentStock = currentStock,
                    StockStatus = currentStock <= 0 ? "Out of Stock" :
                                  (record.Item.ReorderLevel > 0 && currentStock <= record.Item.ReorderLevel) ? "Low Stock" : "In Stock",
                    History = ledger.OrderByDescending(t => t.TransactionDate).Select(t => new InventoryStockTransactionDto
                    {
                        Id = t.Id,
                        InventoryItemId = t.InventoryItemId,
                        TransactionDate = t.TransactionDate,
                        TransactionType = t.TransactionType,
                        TransactionTypeName = t.TransactionType.ToString(),
                        Quantity = t.Quantity,
                        SupplierId = t.SupplierId,
                        SupplierName = t.Supplier?.Name,
                        ReferenceType = t.ReferenceType,
                        ReferenceId = t.ReferenceId,
                        Notes = t.Notes,
                        CreatedAt = t.CreatedAt,
                        CreatedBy = t.CreatedBy,
                        InventoryBatchId = t.InventoryBatchId,
                        BatchNumber = record.Batches.FirstOrDefault(b => b.Id == t.InventoryBatchId)?.BatchNumber
                    }).ToList(),
                    Batches = batchDtos.OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue).ToList()
                });
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task<InventoryItem?> GetItemByIdAsync(Guid id)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task<InventoryItemDto?> GetItemDtoByIdAsync(Guid id)
        {
            var allItems = await GetAllItemsAsync();
            return allItems.FirstOrDefault(i => i.Id == id);
        }

        public async Task<InventoryBatch> CreateBatchAsync(InventoryBatch batch)
        {
            batch.CreatedAt = DateTime.UtcNow;
            _context.InventoryBatches.Add(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<InventoryItem> CreateItemAsync(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateItemAsync(InventoryItem item)
        {
            item.UpdatedAt = DateTime.UtcNow;
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<InventoryStockTransaction> AddStockTransactionAsync(InventoryStockTransaction transaction)
        {
            // Negative stock protection
            var ledger = await _context.InventoryStockTransactions
                .Where(t => t.InventoryItemId == transaction.InventoryItemId)
                .ToListAsync();

            decimal currentStock = 0;
            decimal batchStock = 0;
            foreach (var tx in ledger)
            {
                bool isIncrease = tx.TransactionType == InventoryTransactionType.OpeningBalance ||
                                  tx.TransactionType == InventoryTransactionType.StockIn ||
                                  tx.TransactionType == InventoryTransactionType.AdjustmentIncrease;
                bool isDecrease = tx.TransactionType == InventoryTransactionType.StockOut ||
                                  tx.TransactionType == InventoryTransactionType.AdjustmentDecrease;

                if (isIncrease) currentStock += tx.Quantity;
                else if (isDecrease) currentStock -= tx.Quantity;

                if (transaction.InventoryBatchId.HasValue && tx.InventoryBatchId == transaction.InventoryBatchId.Value)
                {
                    if (isIncrease) batchStock += tx.Quantity;
                    else if (isDecrease) batchStock -= tx.Quantity;
                }
            }

            decimal projectedStock = currentStock;
            decimal projectedBatchStock = batchStock;
            
            bool isNewIncrease = transaction.TransactionType == InventoryTransactionType.OpeningBalance ||
                                 transaction.TransactionType == InventoryTransactionType.StockIn ||
                                 transaction.TransactionType == InventoryTransactionType.AdjustmentIncrease;
            bool isNewDecrease = transaction.TransactionType == InventoryTransactionType.StockOut ||
                                 transaction.TransactionType == InventoryTransactionType.AdjustmentDecrease;

            if (isNewIncrease)
            {
                projectedStock += transaction.Quantity;
                projectedBatchStock += transaction.Quantity;
            }
            else if (isNewDecrease)
            {
                projectedStock -= transaction.Quantity;
                projectedBatchStock -= transaction.Quantity;
            }

            if (projectedStock < 0)
            {
                throw new InvalidOperationException("Transaction rejected: resulting item stock would be negative.");
            }
            
            if (transaction.InventoryBatchId.HasValue && projectedBatchStock < 0)
            {
                throw new InvalidOperationException("Transaction rejected: resulting batch stock would be negative.");
            }

            transaction.TransactionDate = DateTime.UtcNow;
            transaction.CreatedAt = DateTime.UtcNow;
            
            _context.InventoryStockTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            
            return transaction;
        }

        public async Task<StockMovementReportDto> GetStockMovementReportAsync(StockMovementFilterDto filter)
        {
            var report = new StockMovementReportDto();
            
            // 1. Fetch available items for the filter dropdown
            var allItems = await GetAllItemsAsync();
            report.AvailableItems = allItems;
            
            // 2. Query transactions based on filter
            var query = _context.InventoryStockTransactions
                .Include(t => t.InventoryItem)
                .Include(t => t.Supplier)
                .Include(t => t.InventoryBatch)
                .AsQueryable();
                
            // Ensure we only query for non-deleted items
            query = query.Where(t => !t.InventoryItem.IsDeleted);

            if (filter.InventoryItemId.HasValue)
            {
                query = query.Where(t => t.InventoryItemId == filter.InventoryItemId.Value);
            }
            if (filter.TransactionType.HasValue)
            {
                query = query.Where(t => t.TransactionType == filter.TransactionType.Value);
            }
            if (filter.DateFrom.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= filter.DateFrom.Value);
            }
            if (filter.DateTo.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= filter.DateTo.Value);
            }

            var transactions = await query.OrderByDescending(t => t.TransactionDate).ToListAsync();

            // Calculate totals from filtered transactions
            foreach (var tx in transactions)
            {
                report.Transactions.Add(new StockMovementTransactionDto
                {
                    Id = tx.Id,
                    Date = tx.TransactionDate,
                    ItemCode = tx.InventoryItem.Code,
                    ItemName = tx.InventoryItem.Name,
                    TransactionType = tx.TransactionType.ToString(),
                    Quantity = tx.Quantity,
                    Notes = tx.Notes,
                    CreatedBy = tx.CreatedBy?.ToString(), // Assuming Guid?
                    SupplierId = tx.SupplierId,
                    SupplierName = tx.Supplier?.Name,
                    SupplierCode = tx.Supplier?.Code,
                    BatchNumber = tx.InventoryBatch?.BatchNumber,
                    ExpiryDate = tx.InventoryBatch?.ExpiryDate
                });

                switch (tx.TransactionType)
                {
                    case InventoryTransactionType.StockIn:
                        report.Summary.TotalStockIn += tx.Quantity;
                        break;
                    case InventoryTransactionType.StockOut:
                        report.Summary.TotalStockOut += tx.Quantity;
                        break;
                    case InventoryTransactionType.AdjustmentIncrease:
                        report.Summary.TotalAdjustmentIncrease += tx.Quantity;
                        break;
                    case InventoryTransactionType.AdjustmentDecrease:
                        report.Summary.TotalAdjustmentDecrease += tx.Quantity;
                        break;
                }
            }

            // Fill item statuses and total items (based on filtered list if item is filtered, or all active if not)
            var itemsToSummarize = filter.InventoryItemId.HasValue 
                ? allItems.Where(i => i.Id == filter.InventoryItemId.Value).ToList()
                : allItems;

            report.Summary.TotalItems = itemsToSummarize.Count;
            foreach (var item in itemsToSummarize)
            {
                report.ItemStatuses.Add(new StockMovementItemStatusDto
                {
                    ItemId = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    CurrentStock = item.CurrentStock,
                    Status = item.StockStatus
                });
            }

            return report;
        }

        public async Task<List<InventoryExpiryMonitoringDto>> GetExpiryMonitoringAsync(int nearExpiryThresholdDays)
        {
            var today = DateTime.UtcNow.Date;
            
            var items = await _context.InventoryItems
                .Where(i => !i.IsDeleted)
                .Select(i => new
                {
                    Item = i,
                    StockLedger = _context.InventoryStockTransactions
                        .Where(t => t.InventoryItemId == i.Id)
                        .ToList(),
                    Batches = _context.InventoryBatches
                        .Where(b => b.InventoryItemId == i.Id && !b.IsDeleted)
                        .ToList()
                })
                .ToListAsync();

            var result = new List<InventoryExpiryMonitoringDto>();

            foreach (var record in items)
            {
                foreach (var batch in record.Batches)
                {
                    decimal batchStock = 0;
                    foreach(var tx in record.StockLedger.Where(t => t.InventoryBatchId == batch.Id)) 
                    {
                        if (tx.TransactionType == InventoryTransactionType.OpeningBalance ||
                            tx.TransactionType == InventoryTransactionType.StockIn ||
                            tx.TransactionType == InventoryTransactionType.AdjustmentIncrease)
                        {
                            batchStock += tx.Quantity;
                        }
                        else if (tx.TransactionType == InventoryTransactionType.StockOut ||
                                 tx.TransactionType == InventoryTransactionType.AdjustmentDecrease)
                        {
                            batchStock -= tx.Quantity;
                        }
                    }
                    
                    string status;
                    int? daysToExpiry = null;
                    
                    if (!batch.ExpiryDate.HasValue)
                    {
                        status = "No Expiry";
                    }
                    else
                    {
                        daysToExpiry = (batch.ExpiryDate.Value.Date - today).Days;
                        if (daysToExpiry < 0)
                        {
                            status = "Expired";
                        }
                        else if (daysToExpiry <= nearExpiryThresholdDays)
                        {
                            status = "Near Expiry";
                        }
                        else
                        {
                            status = "Active";
                        }
                    }

                    result.Add(new InventoryExpiryMonitoringDto
                    {
                        BatchId = batch.Id,
                        ItemId = record.Item.Id,
                        ItemCode = record.Item.Code,
                        ItemName = record.Item.Name,
                        BatchNumber = batch.BatchNumber,
                        ExpiryDate = batch.ExpiryDate,
                        DaysToExpiry = daysToExpiry,
                        CurrentStock = batchStock,
                        ExpiryStatus = status
                    });
                }
            }

            return result
                .OrderBy(x => x.ExpiryStatus switch 
                { 
                    "Expired" => 1, 
                    "Near Expiry" => 2, 
                    "Active" => 3, 
                    "No Expiry" => 4, 
                    _ => 5 
                })
                .ThenBy(x => x.ExpiryDate ?? DateTime.MaxValue)
                .ToList();
        }
    }
}

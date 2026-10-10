using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;
using Clinic.Application.DTOs.Operations;
using Clinic.Application.UI;

namespace Clinic.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly Clinic.Application.Interfaces.Configuration.IAppConfigurationService _appConfigurationService;
        private readonly Clinic.Application.Interfaces.MasterData.ISupplierRepository _supplierRepository;

        public InventoryController(IInventoryRepository inventoryRepository, Clinic.Application.Interfaces.Configuration.IAppConfigurationService appConfigurationService, Clinic.Application.Interfaces.MasterData.ISupplierRepository supplierRepository)
        {
            _inventoryRepository = inventoryRepository;
            _appConfigurationService = appConfigurationService;
            _supplierRepository = supplierRepository;
        }

        [HttpGet]
        [Authorize(Policy = "Inventory.Index")]
        public async Task<IActionResult> Index()
        {
            var meta = new UIMetadata
            {
                Title = "Inventory",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            var items = await _inventoryRepository.GetAllItemsAsync();
            var suppliers = await _supplierRepository.GetAllAsync();
            
            ViewBag.Meta = meta;
            ViewBag.ActiveSuppliers = suppliers.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
            return View("Templates/Inventory_List", items);
        }

        [HttpGet("Create")]
        [Authorize(Policy = "Inventory.Index")] // Reuse existing permission
        public IActionResult Create()
        {
            var meta = new UIMetadata
            {
                Title = "Create Inventory Item",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            return View("Templates/Inventory_ItemForm", new InventoryItemDto());
        }

        [HttpPost("Create")]
        [Authorize(Policy = "Inventory.Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItemDto dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventoryItem
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Category = dto.Category,
                    Unit = dto.Unit,
                    ReorderLevel = dto.ReorderLevel,
                    IsActive = dto.IsActive
                };
                await _inventoryRepository.CreateItemAsync(entity);
                return RedirectToAction("Index");
            }
            var meta = new UIMetadata { Title = "Create Inventory Item", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Inventory_ItemForm", dto);
        }

        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "Inventory.Index")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _inventoryRepository.GetItemByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new InventoryItemDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Category = entity.Category,
                Unit = entity.Unit,
                ReorderLevel = entity.ReorderLevel,
                IsActive = entity.IsActive
            };
            
            var meta = new UIMetadata { Title = "Edit Inventory Item", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Inventory_ItemForm", dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Policy = "Inventory.Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, InventoryItemDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var entity = await _inventoryRepository.GetItemByIdAsync(id);
                if (entity == null) return NotFound();

                entity.Code = dto.Code;
                entity.Name = dto.Name;
                entity.Category = dto.Category;
                entity.Unit = dto.Unit;
                entity.ReorderLevel = dto.ReorderLevel;
                entity.IsActive = dto.IsActive;

                await _inventoryRepository.UpdateItemAsync(entity);
                return RedirectToAction("Index");
            }
            
            var meta = new UIMetadata { Title = "Edit Inventory Item", ModuleName = "Inventory", Mode = RenderingMode.Template };
            ViewBag.Meta = meta;
            return View("Templates/Inventory_ItemForm", dto);
        }

        // Opening Balance POST logic
        [HttpPost("OpeningBalance")]
        [Authorize(Policy = "Inventory.Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpeningBalance(Guid id, decimal quantity, string? notes)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than 0.";
                return RedirectToAction("Index");
            }

            var transaction = new InventoryStockTransaction
            {
                InventoryItemId = id,
                TransactionType = InventoryTransactionType.OpeningBalance,
                Quantity = quantity,
                Notes = notes
            };

            await _inventoryRepository.AddStockTransactionAsync(transaction);
            
            TempData["Success"] = "Opening balance applied successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost("StockOperation")]
        [Authorize(Policy = "Inventory.Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOperation(StockOperationDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid transaction parameters.";
                return RedirectToAction("Index");
            }

            if (dto.TransactionType == InventoryTransactionType.AdjustmentIncrease || 
                dto.TransactionType == InventoryTransactionType.AdjustmentDecrease)
            {
                if (string.IsNullOrWhiteSpace(dto.Notes))
                {
                    TempData["Error"] = "Adjustment transactions require a reason/notes.";
                    return RedirectToAction("Index");
                }
            }

            var transaction = new InventoryStockTransaction
            {
                InventoryItemId = dto.InventoryItemId,
                InventoryBatchId = dto.InventoryBatchId,
                SupplierId = dto.SupplierId,
                TransactionType = dto.TransactionType,
                Quantity = dto.Quantity,
                TransactionDate = dto.TransactionDate,
                Notes = dto.Notes
            };

            try
            {
                await _inventoryRepository.AddStockTransactionAsync(transaction);
                TempData["Success"] = "Stock transaction successful.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while processing the transaction.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet("StockMovement")]
        [Authorize(Policy = "Inventory.Index")]
        public async Task<IActionResult> StockMovement([FromQuery] StockMovementFilterDto filter)
        {
            var report = await _inventoryRepository.GetStockMovementReportAsync(filter);
            
            var meta = new UIMetadata
            {
                Title = "Stock Movement",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            return View("Templates/Inventory_StockMovement", report);
        }
        [HttpGet("Batches/{id}")]
        [Authorize(Policy = "Inventory.Index")]
        public async Task<IActionResult> Batches(Guid id)
        {
            var dto = await _inventoryRepository.GetItemDtoByIdAsync(id);
            if (dto == null) return NotFound();
            
            var meta = new UIMetadata
            {
                Title = $"Manage Batches - {dto.Name}",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;

            return View("Templates/Inventory_Batches", dto);
        }

        [HttpPost("CreateBatch")]
        [Authorize(Policy = "Inventory.Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBatch(Guid inventoryItemId, string batchNumber, DateTime? expiryDate)
        {
            if (string.IsNullOrWhiteSpace(batchNumber))
            {
                TempData["Error"] = "Batch Number is required.";
                return RedirectToAction("Batches", new { id = inventoryItemId });
            }

            try
            {
                var batch = new InventoryBatch
                {
                    InventoryItemId = inventoryItemId,
                    BatchNumber = batchNumber,
                    ExpiryDate = expiryDate,
                    IsActive = true
                };

                await _inventoryRepository.CreateBatchAsync(batch);
                TempData["Success"] = "Batch created successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to create batch. Batch number must be unique per item.";
            }

            return RedirectToAction("Batches", new { id = inventoryItemId });
        }

        [HttpGet("ExpiryMonitoring")]
        [Authorize(Policy = "Inventory.Index")]
        public async Task<IActionResult> ExpiryMonitoring(string statusFilter = "")
        {
            int threshold = await _appConfigurationService.GetIntValueAsync("Inventory.NearExpiryThresholdDays", 30);
            
            var items = await _inventoryRepository.GetExpiryMonitoringAsync(threshold);
            
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                items = items.Where(x => x.ExpiryStatus == statusFilter).ToList();
            }

            var meta = new UIMetadata
            {
                Title = "Expiry & Near-Expiry Monitoring",
                ModuleName = "Inventory",
                Mode = RenderingMode.Template
            };
            ViewBag.Meta = meta;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.Threshold = threshold;

            return View("Templates/Inventory_ExpiryMonitoring", items);
        }
    }
}

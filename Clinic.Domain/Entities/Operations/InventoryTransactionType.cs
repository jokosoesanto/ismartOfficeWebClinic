namespace Clinic.Domain.Entities.Operations
{
    public enum InventoryTransactionType
    {
        OpeningBalance = 1,
        StockIn = 2,
        StockOut = 3,
        AdjustmentIncrease = 4,
        AdjustmentDecrease = 5
    }
}

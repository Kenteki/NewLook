using NewLook.Models.DTOs.Api;

namespace NewLook.Services.Interfaces;

public interface IInventoryAggregationService
{
    Task<InventoryAggregatedDataDto> GetAggregatedDataAsync(int inventoryId);
}

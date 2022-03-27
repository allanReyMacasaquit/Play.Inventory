using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDto(
        Guid UserId,
        Guid CatalogItemId,
        int Quantity
    );
    public record InventoryItemDto(
        string Name,
        string Description,
        int Quantity,
        Guid CatalogItemId,
        DateTimeOffset AcquiredDate
    );
    public record CatalogItemDto(
        Guid Id,
        string Name,
        string Description
    );
}
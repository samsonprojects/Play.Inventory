using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Inventory.Service
{
    public class Dtos
    {
        public record GrantItemsDto(Guid userId, Guid CatalogItemId, int Quantity);
        public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
        public record CatalogItemDto(Guid Id, string Name, string Description);
    }
}
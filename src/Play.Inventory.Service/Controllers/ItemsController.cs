using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _itemsRepository;
        private readonly CatalogClient _catalogClient;
        public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
        {
            _itemsRepository = itemsRepository;
            _catalogClient = catalogClient;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var catalogItems = await _catalogClient.GetCatalogItemDtosAsync();
            var inventoryItemEntities = await _itemsRepository.GetAllAsync(item => item.userId == userId);

            var items = (await _itemsRepository.GetAllAsync(item => item.userId == userId)).Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(items);

        }

        [HttpPost]

        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {

            var foundInventoryItem = await _itemsRepository.GetAsync(item => item.CatalogItemId == grantItemsDto.CatalogItemId && grantItemsDto.userId == item.userId);
            if (foundInventoryItem == null)
            {
                var inventoryItem = new InventoryItem()
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    userId = grantItemsDto.userId,
                    Quantity = grantItemsDto.Quantity
                    ,
                    AcquiredDate = DateTimeOffset.UtcNow

                };
                await _itemsRepository.CreateAsync(inventoryItem);
                return Ok();
            }
            foundInventoryItem.Quantity += grantItemsDto.Quantity;
            await _itemsRepository.UpdateAsync(foundInventoryItem);
            return Ok();

        }



    }
}
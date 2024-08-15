using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;

        private readonly IRepository<CatalogItem> _catalogItemsRepository;
        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(item => item.userId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var items = (await _inventoryItemsRepository.GetAllAsync(item => item.userId == userId)).Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(items);

        }

        [HttpPost]

        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {

            var foundInventoryItem = await _inventoryItemsRepository.GetAsync(item => item.CatalogItemId == grantItemsDto.CatalogItemId && grantItemsDto.userId == item.userId);
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
                await _inventoryItemsRepository.CreateAsync(inventoryItem);
                return Ok();
            }
            foundInventoryItem.Quantity += grantItemsDto.Quantity;
            await _inventoryItemsRepository.UpdateAsync(foundInventoryItem);
            return Ok();

        }



    }
}
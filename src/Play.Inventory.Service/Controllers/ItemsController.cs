using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;

        private readonly IRepository<CatalogItem> _catalogItemsRepository;
        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
        }



        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (Guid.Parse(currentUserId) != userId)
            {
                if (!User.IsInRole(AdminRole))
                {
                    return Forbid();
                }
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
        [Authorize(Roles = AdminRole)]
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
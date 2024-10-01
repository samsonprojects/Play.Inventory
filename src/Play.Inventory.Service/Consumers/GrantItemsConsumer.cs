using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Common.Service.Repositories;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;

namespace Play.Inventory.Service.Consumers
{
    public class GrantItemsConsumer : IConsumer<GrantItems>
    {
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;

        private readonly IRepository<CatalogItem> _catalogItemsRepository;

        public GrantItemsConsumer(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
        }
        public async Task Consume(ConsumeContext<GrantItems> context)
        {
            var message = context.Message;

            var item = await _catalogItemsRepository.GetAsync(message.CatalogItemId);

            if (item == null)
            {
                throw new UnknownItemException(message.CatalogItemId);
            }
            var foundInventoryItem = await _inventoryItemsRepository.GetAsync(item => item.CatalogItemId == message.CatalogItemId && message.UserId == item.userId);
            if (foundInventoryItem == null)
            {
                var inventoryItem = new InventoryItem()
                {
                    CatalogItemId = message.CatalogItemId,
                    userId = message.UserId,
                    Quantity = message.Quantity
                    ,
                    AcquiredDate = DateTimeOffset.UtcNow

                };
                await _inventoryItemsRepository.CreateAsync(inventoryItem);

            }
            else
            {
                foundInventoryItem.Quantity += message.Quantity;
                await _inventoryItemsRepository.UpdateAsync(foundInventoryItem);
            }


            await context.Publish(new InventoryItemsGranted(message.CorrelationId));
        }
    }
}
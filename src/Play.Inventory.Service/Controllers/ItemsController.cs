using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryitemsRepository;
        private readonly IRepository<CatalogItem> _catalogItemsRepository;

        public ItemsController(
            IRepository<InventoryItem> inventoryitemsRepository,
            IRepository<CatalogItem> catalogItemsRepository)
        {
            _catalogItemsRepository = catalogItemsRepository;
            _inventoryitemsRepository = inventoryitemsRepository;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var inventoryItemEntities = await _inventoryitemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);

            var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
                {
                    var catalogItem = catalogItemEntities.Single(catalogItem =>
                    catalogItem.Id == inventoryItem.CatalogItemId);

                    return inventoryItem.AsDto(
                        catalogItem.Name,
                        catalogItem.Description
                    );
                });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await _inventoryitemsRepository.GetAsync(
                item =>
                item.UserId == grantItemsDto.UserId &&
                item.CatalogItemId == grantItemsDto.CatalogItemId
            );

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.Now
                };

                await _inventoryitemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _inventoryitemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}
using System;
using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> _repositoryDeleted;
        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repositoryDeleted)
        {
            _repositoryDeleted = repositoryDeleted;

        }
        
        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await _repositoryDeleted.GetAsync(message.ItemId);

            if (item == null) return;

            await _repositoryDeleted.RemoveAsync(message.ItemId);

        }
    }
}
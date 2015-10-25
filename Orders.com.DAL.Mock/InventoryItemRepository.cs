using AutoMapper;
using Orders.com.Core.DataProxy;
using Orders.com.Core.Domain;
using Orders.com.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Orders.com.Core.Exceptions;

namespace Orders.com.DAL.Mock
{
    public class InventoryItemRepository : OrdersDotComMockBase<InventoryItem>, IInventoryItemDataProxy
    {
        protected override IEnumerable<InventoryItem> SeedRepository()
        {
            yield return new InventoryItem() { InventoryItemID = 1, ProductID = 1, QuantityOnHand = 50, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 2, ProductID = 2, QuantityOnHand = 1, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 3, ProductID = 3, QuantityOnHand = 4, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 4, ProductID = 4, QuantityOnHand = 3, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 5, ProductID = 5, QuantityOnHand = 20, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 6, ProductID = 6, QuantityOnHand = 54, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 7, ProductID = 7, QuantityOnHand = 12, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 8, ProductID = 8, QuantityOnHand = 10, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 9, ProductID = 9, QuantityOnHand = 34, Version = "1" };
            yield return new InventoryItem() { InventoryItemID = 10, ProductID = 10, QuantityOnHand = 11, Version = "1" };
        }

        public InventoryItem GetByProduct(long productID)
        {
            Debug.WriteLine("Executing EF InventoryItem.GetByProduct");
            var inventoryItem = Data.Values.First(c => c.ProductID == productID);
            return Mapper.Map(inventoryItem, new InventoryItem());
        }

        public InventoryItem DecrementQuantityOnHand(long inventoryID, decimal quantity)
        {
            Debug.WriteLine("DECREMENTING inventoryItem.QuantityOnHand in database");
            //lock (_lockObject)
            //{
                var existing = Data.Values.FirstOrDefault(c => c.ID == inventoryID);
                if (existing == null || quantity > existing.QuantityOnHand)
                    throw new InsufficientStockAmountException(string.Format("There is not enough in stock to fullfill the request"));

                existing.QuantityOnHand -= quantity;
                existing.IncrementVersionByOne();
                return Mapper.Map(existing, new InventoryItem());
            //}
        }

        public async Task<InventoryItem> GetByProductAsync(long productID)
        {
            return GetByProduct(productID);
        }

        public async Task<InventoryItem> DecrementQuantityOnHandAsync(long inventoryID, decimal quantity)
        {
            return DecrementQuantityOnHand(inventoryID, quantity);
        }
    }
}

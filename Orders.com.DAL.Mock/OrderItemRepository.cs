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

namespace Orders.com.DAL.Mock
{
    public class OrderItemRepository : OrdersDotComMockBase<OrderItem>, IOrderItemDataProxy 
    {
        public IEnumerable<OrderItem> GetByOrder(long orderID)
        {
            Debug.WriteLine("Executing EF OrderItem.GetByOrder");
            return Data.Values.Where(i => i.OrderID == orderID)
                              .Select(Mapper.Map<OrderItem, OrderItem>)
                              .ToArray();
        }

        public OrderItem BackOrder(long orderItemID, DateTime backOrderedOn)
        {
            Debug.WriteLine("UPDATING orderItem in database - backordered state");
            var existing = Data[orderItemID];
            existing.OrderStatus().SetBackorderedState();
            existing.BackorderedDate = backOrderedOn;
            return Mapper.Map(existing, new OrderItem());
        }

        public OrderItem Submit(long orderItemID, DateTime submittedOn)
        {
            Debug.WriteLine("UPDATING orderItem in database - submitted state");
            var existing = Data[orderItemID];
            existing.OrderStatus().SetSubmittedState();
            existing.SubmittedDate = submittedOn;
            return Mapper.Map(existing, new OrderItem());
        }

        public OrderItem Ship(long orderItemID, DateTime shippedOn)
        {
            Debug.WriteLine("UPDATING orderItem in database - shipped state");
            var existing = Data[orderItemID];
            existing.OrderStatus().SetShippedState();
            existing.ShippedDate = shippedOn;
            return Mapper.Map(existing, new OrderItem());
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderAsync(long orderID)
        {
            return GetByOrder(orderID);
        }

        public async Task<OrderItem> BackOrderAsync(long orderItemID, DateTime backOrderedOn)
        {
            return BackOrder(orderItemID, backOrderedOn);
        }

        public async Task<OrderItem> SubmitAsync(long orderItemID, DateTime shippedOn)
        {
            return Submit(orderItemID, shippedOn);
        }

        public async Task<OrderItem> ShipAsync(long orderItemID, DateTime shippedOn)
        {
            return Ship(orderItemID, shippedOn);
        }
    }
}

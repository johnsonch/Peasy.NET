﻿using AutoMapper;
using Orders.com.Core.DataProxy;
using Orders.com.Core.Domain;
using Orders.com.Core.Extensions;
using Orders.com.Core.QueryData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.com.DAL.Mock
{
    public class OrderRepository : OrdersDotComMockBase<Order>, IOrderDataProxy
    {
        private List<Order> _orders;
        private ICustomerDataProxy _customerDataProxy;
        private IOrderItemDataProxy _orderItemDataProxy;

        public OrderRepository(ICustomerDataProxy customerDataProxy, IOrderItemDataProxy orderItemDataProxy)
        {
            Mapper.CreateMap<Order, Order>();
            _customerDataProxy = customerDataProxy;
            _orderItemDataProxy = orderItemDataProxy;
        }

        protected override IEnumerable<Order> SeedRepository()
        {
            yield return new Order() { OrderID = 1, CustomerID = 1, OrderDate = DateTime.Now.AddMonths(-3) };
        }

        public IEnumerable<OrderInfo> GetAll(int start, int pageSize)
        {
            var orders = GetAll();
            var customers = _customerDataProxy.GetAll().ToDictionary(c => c.CustomerID);
            var orderItems = _orderItemDataProxy.GetAll().ToArray();
            var results = orders.Skip(start)
                                .Take(pageSize)
                                .Select(o => new
                                {
                                    OrderID = o.OrderID,
                                    OrderDate = o.OrderDate,
                                    CustomerName = customers[o.CustomerID].Name,
                                    CustomerID = o.CustomerID,
                                    OrderItems = orderItems.Where(i => i.OrderID == o.OrderID)
                                })
                                .Select(o => new OrderInfo()
                                {
                                    OrderID = o.OrderID,
                                    OrderDate = o.OrderDate,
                                    CustomerName = o.CustomerName,
                                    CustomerID = o.CustomerID,
                                    Total = o.OrderItems.Sum(i => i.Amount),
                                    Status = BuildStatusName(o.OrderItems),
                                    HasShippedItems = o.OrderItems.Any(i => i.OrderStatus() is ShippedState)
                                });
            return results.ToArray();
        }

        private static string BuildStatusName(IEnumerable<OrderItem> orderItems)
        {
            if (!orderItems.Any()) return string.Empty;
            return orderItems.OrderStatus().Name;
        }

        public IEnumerable<Order> GetByCustomer(long customerID)
        {
            Debug.WriteLine("Executing EF Order.GetByCustomer");
            return Data.Values.Where(o => o.CustomerID == customerID)
                              .Select(Mapper.Map<Order, Order>).ToArray();
        }

        public IEnumerable<Order> GetByProduct(long productID)
        {
            Debug.WriteLine("Executing EF Order.GetByProduct");
            var orderItems = new OrderItemRepository().GetAll().ToArray();

            return Data.Values.Where(o => orderItems.Any(i => i.OrderID == o.OrderID &&
                                                         i.ProductID == productID))
                              .Select(Mapper.Map<Order, Order>).ToArray();
        }

        public async Task<IEnumerable<Order>> GetByProductAsync(long productID)
        {
            return GetByProduct(productID);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(long customerID)
        {
            return GetByCustomer(customerID);
        }

        public async Task<IEnumerable<OrderInfo>> GetAllAsync(int start, int pageSize)
        {
            return GetAll(start, pageSize);
        }
    }
}
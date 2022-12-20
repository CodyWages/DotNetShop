﻿using Shop.Domain.Enums;
using Shop.Domain.Models;

namespace Shop.Domain.Infrastructure
{
    public interface IOrderManager
    {
        bool OrderReferenceExists(string reference);

        IEnumerable<TResult> GetOrdersByStatus<TResult>(OrderStatus status, Func<Order, TResult> selector);
        TResult GetOrderById<TResult>(int id, Func<Order, TResult> selector);
        TResult GetOrderByReference<TResult>(string reference, Func<Order, TResult> selector);

        Task<int> CreateOrder(Order order);
        Task AdvanceOrder(int id);
    }
}

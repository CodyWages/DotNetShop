using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Shop.Domain.Enums;
using Shop.Domain.Infrastructure;
using Shop.Domain.Models;

namespace Shop.Database
{
    public class OrderManager : IOrderManager
    {
        private readonly ApplicationDbContext _ctx;

        public OrderManager(ApplicationDbContext ctx) 
        {
            _ctx = ctx;
        }

        public bool OrderReferenceExists(string reference)
        {
            return _ctx.Orders.Any(x => x.OrderRef == reference);
        }

        public IEnumerable<TResult> GetOrdersByStatus<TResult>(OrderStatus status, Func<Order, TResult> selector)
        {
            return _ctx.Orders
                .Where(x => x.Status == status)
                .Select(selector)
                .ToList();
        }

        private TResult GetOrder<TResult>(
            Func<Order, bool> condition,
            Func<Order, TResult> selector)
        {
            return _ctx.Orders
                .Where(x => condition(x))
                .Include(x => x.OrderStocks)
                    .ThenInclude(x => x.Stock)
                        .ThenInclude(x => x.Product)
                .Select(selector)
                .FirstOrDefault();
        }

        public TResult GetOrderById<TResult>(int id, Func<Order, TResult> selector)
        {
            return GetOrder(
                order => order.Id == id, selector);
        }

        public TResult GetOrderByReference<TResult>(
            string reference, 
            Func<Order, TResult> selector)
        {
            return _ctx.Orders
                .Where(x => x.OrderRef == reference)
                .Include(x => x.OrderStocks)
                    .ThenInclude(x => x.Stock)
                        .ThenInclude(x => x.Product)
                .Select(selector)
                .FirstOrDefault();
        }

        public Task<int> CreateOrder(Order order)
        {
            _ctx.Orders.Add(order);

            return _ctx.SaveChangesAsync();
        }

        public Task<int> AdvanceOrder(int id)
        {
            _ctx.Orders.FirstOrDefault(x => x.Id == id).Status++;

            return _ctx.SaveChangesAsync();
        }
    }
}

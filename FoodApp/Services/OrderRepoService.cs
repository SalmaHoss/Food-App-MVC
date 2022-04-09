using FoodApp.Data.Cart;
using FoodApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodApp.Services
{
    public class OrderRepoService : IOrderRepository
    {
        private readonly AppDbContext context;
        public OrderRepoService(AppDbContext _context)
        {
            context = _context;
        }
        //public List<Order> GetOrderByUserIdAsync(string userId)
        //{
        //    var orders =  context.Orders.Include(n=>n.OrderItems).ThenInclude(n=>n.Item)
        //        .Where(n=>n.UserId == userId).ToList();

        //    return orders;
        //        }

        //public void StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        //{

        //    //1
        //    //2
        //    var order = new Order()
        //    {
        //        UserId = userId,
        //        Email = userEmailAddress
        //    };
        //     context.Orders.Add(order);
        //     context.SaveChanges();

        //    foreach(var item in items)
        //    {
        //        var orderItem = new OrderItem()
        //        {
        //            Amount = item.Amount,
        //            ItemId = item.Item.Id,
        //            OrderId = order.Id,
        //            Price = item.Item.Price
        //        };
        //         context.OrderItems.Add(orderItem);
        //    }
        //        context.SaveChanges();
        //}
        public async Task<List<Order>> GetOrderByUserIdRoleAsync(string userId,string userRole)
        {

            //get User orders
            //var orders = await context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Item)
            //    .Where(n => n.UserId == userId).ToListAsync();

            //get all User orders
            var orders = await context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Item).Include(n => n.User).ToListAsync();

            if (userRole != "Admin")
            {
                orders = orders.Where(n => n.UserId == userId).ToList();
            }

            return orders;

        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress
            };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    ItemId = item.Item.Id,
                    OrderId = order.Id,
                    Price = item.Item.Price
                };
                await context.OrderItems.AddAsync(orderItem);
            }
            await context.SaveChangesAsync();
        }
    }
}

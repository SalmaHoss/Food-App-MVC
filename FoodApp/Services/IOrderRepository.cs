using FoodApp.Data.Cart;
using FoodApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp.Services
{
    public interface IOrderRepository
    {

        Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress);
        Task<List<Order>> GetOrderByUserIdRoleAsync(string userId,string userRole);
    }
}

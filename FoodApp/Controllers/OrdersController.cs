using FoodApp.Data.Cart;
using FoodApp.Data.ViewModel;
using FoodApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IItemRepository itemRepository;
        private readonly ShoppingCart shoppingCart;
        private readonly IOrderRepository orderRepository;
        public OrdersController(IItemRepository _itemRepository, ShoppingCart _shoppingCart, IOrderRepository _orderRepository)
        {
            itemRepository = _itemRepository;
            shoppingCart = _shoppingCart;
            orderRepository = _orderRepository;
        }
        
  
        public async Task<IActionResult> Index()
        {
            string userId = "";//User.FindFirstValue(ClaimTypes.NameIdentifier);
            //string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await orderRepository.GetOrderByUserIdAsync(userId);
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
           var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;
            var response = new ShoppingCartVM()
            {
                ShoppingCart=shoppingCart,
                ShoppingCartTotal=shoppingCart.GetShoppingCartTotal()
            };
            return View(response);
        }

        public async Task<IActionResult> AddToShoppingCart(int id)
        {
            var item = await itemRepository.GetDetails(id);

            if(item != null)
            {
                shoppingCart.AddItemToCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));

        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await itemRepository.GetDetails(id);

            if (item != null)
            {
                shoppingCart.RemoveItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));

        }

        public IActionResult CompleteOrder()
        {
            var items = shoppingCart.GetShoppingCartItems();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

             orderRepository.StoreOrderAsync(items, userId, userEmailAddress);
             shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }

    }
}

using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FoodApp.Controllers
{
    public class ItemController : Controller
    {
        public IItemRepository ItemRepository { get; set; }
        public IRestaurantRepsitory RestaurantRepsitory { get; set; }

        public ItemController(IItemRepository itemRepository,IRestaurantRepsitory restaurantRepsitory)
        {
            ItemRepository = itemRepository;
            RestaurantRepsitory = restaurantRepsitory;
        }
        // GET: ItemController
        public ActionResult Index()
        {
            return View(ItemRepository.GetAll());
        }

        // GET: ItemController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var DetView = await ItemRepository.GetDetails(id);
            return View(DetView);
        }

        // GET: ItemController/Create
        public ActionResult Create()
        {
            ViewData["RestId"] = new SelectList(RestaurantRepsitory.GetAll(), "Id", "Name");
            return View();
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item item)
        {
            try
            {
                ItemRepository.Insert(item);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemController/Edit/5
        public ActionResult Edit(int id)

        {
            ViewData["RestId"] = new SelectList(RestaurantRepsitory.GetAll(), "Id", "Name");
            return View(ItemRepository.GetDetails(id));
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,Item item)
        {
            try
            {
                ItemRepository.Update(id, item);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemController/Delete/5
        public ActionResult Delete(int id)
        {
            
            return View(ItemRepository.GetDetails(id));
        }

        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,Item item)
        {
            try
            {
                ItemRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

using FoodApp.Data;
using FoodApp.Data.ViewModel;
using FoodApp.Models;
using FoodApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FoodApp.Controllers
{
    public class RestaurantController : Controller
    {
        public IRestaurantRepsitory RestaurantRepsitory { get; set; }

        // GET: RestaurantController

        public RestaurantController(IRestaurantRepsitory restaurantRepsitory)
        {
            RestaurantRepsitory = restaurantRepsitory;
        }
       
        [HttpGet]
        public ActionResult Index()
        {
            ViewData["rest"] = RestaurantRepsitory.GetAll();
            ViewData["login"] = new LoginVM();
            return View();
        }

        // GET: RestaurantController/Details/5
        [HttpPost]
        public ActionResult Index(string[] category,string[] city)
        {
            //Modify

            List<Restaurant> restCatCity = new List<Restaurant>();
            foreach (var chk in category)
            {
               var restCat = RestaurantRepsitory.GetAll().FindAll(x => x.Description.ToString() == chk);
                restCatCity.AddRange(restCat);
            }
            List<Restaurant> restCity = restCatCity;

            foreach (var chk in city)
            {
                restCatCity = restCatCity.FindAll(x => x.government.ToString() == chk);

            }
            ViewData["rest"] = restCatCity;
            ViewData["login"] = new LoginVM();
            return View();
        }

        public ActionResult Details(int id)
        {
            var error = RestaurantRepsitory.GetDetails(id);
            if(error == null)
            {
                return View("NotFound");
            }
            return View(RestaurantRepsitory.GetDetails(id));
            
        }

        // GET: RestaurantController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurant restaurant)
        {
            try
            {
                RestaurantRepsitory.Insert(restaurant);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RestaurantController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(RestaurantRepsitory.GetDetails(id));
        }

        // POST: RestaurantController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Restaurant restaurant)
        {
            try
            {
                RestaurantRepsitory.Update(id, restaurant);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RestaurantController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(RestaurantRepsitory.GetDetails(id));
        }

        // POST: RestaurantController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Restaurant restaurant)
        {
            try
            {
                RestaurantRepsitory.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

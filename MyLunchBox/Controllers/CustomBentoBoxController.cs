using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using MyLunchBox.Customization;

namespace MyLunchBox.Controllers
{ 
    public class CustomBentoBoxController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();


        public ActionResult ChooseRestaurant()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Create;
            var selectedUniversity = LocationHelper.GetSelectedUniversity(HttpContext);
            if (selectedUniversity != null)
            {
                var restaurants = db.Restaurants.Where(i=>i.UniversityId == selectedUniversity.UniversityId);
                return View(restaurants);
            }
            return View(new List<Restaurant>());
        }

        public ActionResult ChooseBentoBox( int restaurantId )
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Create;
            var restaurant = db.Restaurants.Single(i => i.RestaurantId == restaurantId);
            if (restaurant != null)
            {
                var bentoBoxes = restaurant.BentoBoxes.Where(i=>i.BentoBoxStatusId == (int)BentoBoxStatusLevel.Available);
                ViewBag.RestaurantName = restaurant.RestaurantName;
                ViewBag.RestaurantImageUrl = restaurant.RestaurantImageUrl;
                ViewBag.RestaurantHoursString = restaurant.RestaurantHoursString;
                return View(bentoBoxes);
            }
            return HttpNotFound();
        }
        //
        // GET: /CustomBentoBox/Create

        public ActionResult Create(int customBentoBoxId = 0, int bentoBoxId = 1)
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Create;
            CustomBentoBox customBentoBox;
            if (customBentoBoxId == 0)
            {
                customBentoBox = new CustomBentoBox()
                {
                    BentoBoxId = bentoBoxId,
                    CustomBentoBoxName = "", 
                    BentoBox = db.BentoBoxes.Single(i=>i.BentoBoxId == bentoBoxId)
                };
            }
            else
            {
                customBentoBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == customBentoBoxId);
                ViewData["recipe"] = customBentoBox.EncodedRecipe;
            }
            var bentoBoxType = db.BentoBoxes.Single(i => i.BentoBoxId == customBentoBox.BentoBoxId);
            var bentoBoxViewModel = new CustomBentoBoxViewModel();
            bentoBoxViewModel.RestaurantName = customBentoBox.BentoBox.Restaurant.RestaurantName;
            bentoBoxViewModel.CurrentCustomBentoBox = customBentoBox;
            bentoBoxViewModel.AvailableDishes = new Dictionary<DishType, List<Dish>>();
            var availableDishTypes = CustomBentoBoxHelper.BentoBoxTypeDishTypeMappings[bentoBoxType.BentoBoxType];
            foreach (var dishType in availableDishTypes)
            {
                bentoBoxViewModel.AvailableDishes.Add(dishType,
                    db.Dishes.Where(i => i.DishTypeId == (int)dishType &&
                        i.DishStatusId == (int)DishStatusLevel.Available &&
                        i.RestaurantId == customBentoBox.BentoBox.RestaurantId).ToList());
            }
            return View(bentoBoxViewModel);
        }

      

        public ActionResult AddToCart(FormCollection values)
        {
            int customBentoBoxId = Convert.ToInt32( values["customBentoBoxId"] );
            string encodedRecipe = values["recipe"];
            string customBentoBoxName = values["customBentoBoxName"];
            CustomBentoBox customBentoBox;
            if (customBentoBoxId == 0)
            {
                customBentoBox = new CustomBentoBox();
                var recipe = CustomBentoBox.DecodeRecipe(encodedRecipe);
                customBentoBox.BentoBoxId = recipe.BentoBoxId;
                customBentoBox.CustomBentoBoxName = customBentoBoxName;
                customBentoBox.AddToCustomBentoBox(recipe.DishIds);
                db.CustomBentoBoxes.AddObject(customBentoBox);
                db.SaveChanges();
            }
            else
            {
                customBentoBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == customBentoBoxId);
                if (customBentoBox != null)
                {
                    var recipe = CustomBentoBox.DecodeRecipe(encodedRecipe);
                    customBentoBox.ReplaceRecipe(recipe.DishIds);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            
            
            if (ModelState.IsValid)
            {
                var shoppingCart = ShoppingCartHelper.GetCart(HttpContext);
                shoppingCart.AddToCart( customBentoBox);
                return RedirectToAction("Edit", "ShoppingCart");
            }
            return RedirectToAction("Create");

            
        }

        //
        // POST: /CustomBentoBox/Create

        [HttpPost]
        public ActionResult Create(CustomBentoBox custombentobox)
        {
            if (ModelState.IsValid)
            {
                var shoppingCart = ShoppingCartHelper.GetCart(this);
                shoppingCart.AddToCart(custombentobox);
                db.SaveChanges();
                return RedirectToAction("Edit", "ShoppingCart");
            }

            ViewBag.BentoBoxId = new SelectList(db.BentoBoxes, "BentoBoxId", "BentoBoxName", custombentobox.BentoBoxId);
            return View(custombentobox);
        }

        [HttpPost]
        public ActionResult IsBentoBoxErrorFree(string encodedRecipe)
        {
            CustomBentoBox customBentoBox = new CustomBentoBox(encodedRecipe);
            // check if error free
            var processor = new Processor();
            processor.ProcessRule(customBentoBox);
         
            var mainCourses = from item in customBentoBox.CustomBentoBoxItems
                              where item.Dish.DishTypeId == (int)DishType.MainCourse
                              select new
                              {
                                  dishId = item.DishId,
                                  dishImageUrl = item.Dish.DishImageUrl,
                                  dishName = item.Dish.DishName,
                                  customBentoBoxId = customBentoBox.CustomBentoBoxId
                              };
            var sideDishes = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.SideDish
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             };
            var others = from item in customBentoBox.CustomBentoBoxItems
                         where item.Dish.DishTypeId == (int)DishType.Drink
                         select new
                         {
                             dishId = item.DishId,
                             dishImageUrl = item.Dish.DishImageUrl,
                             dishName = item.Dish.DishName,
                             customBentoBoxId = customBentoBox.CustomBentoBoxId
                         };

            var warningMsg = processor.Errors;
            encodedRecipe = customBentoBox.EncodedRecipe;
            var unitPrice = customBentoBox.BentoBox.UnitPrice + customBentoBox.CustomBentoBoxItems.Sum(i => i.Dish.DishIncrementalPrice * i.Quantity);

            return Json(new
            {
                encodedRecipe = encodedRecipe,
                currentItems =
                    new
                    {
                        mainCourses = new { dishes = mainCourses, count = customBentoBox.BentoBox.NumOfEntree },
                        sideDishes = new { dishes = sideDishes, count = 1 },
                        others = new { dishes = others, count = 1 }
                    },
                warningMsg = warningMsg,
                isValid = processor.Errors.Count == 0,
                unitPrice = unitPrice
            });
        }

        [HttpPost]
        public ActionResult ChangeQuantity(string encodedRecipe, int dishId, int dishQuantity)
        {
            var dish = db.Dishes.Single(i => i.DishId == dishId);
            var customBentoBox = new CustomBentoBox(encodedRecipe);
            customBentoBox.ChangeQuantity(dish, dishQuantity);
            var processor = new Processor();
            processor.ProcessRule(customBentoBox);
            var mainCourses = from item in customBentoBox.CustomBentoBoxItems
                              where item.Dish.DishTypeId == (int)DishType.MainCourse
                              select new
                              {
                                  dishId = item.DishId,
                                  dishImageUrl = item.Dish.DishImageUrl,
                                  dishName = item.Dish.DishName,
                                  customBentoBoxId = customBentoBox.CustomBentoBoxId
                              };
            var sideDishes = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.SideDish
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             };
            var others = from item in customBentoBox.CustomBentoBoxItems
                         where item.Dish.DishTypeId == (int)DishType.Drink
                         select new
                         {
                             dishId = item.DishId,
                             dishImageUrl = item.Dish.DishImageUrl,
                             dishName = item.Dish.DishName,
                             customBentoBoxId = customBentoBox.CustomBentoBoxId
                         };

            var unitPrice = customBentoBox.BentoBox.UnitPrice + customBentoBox.CustomBentoBoxItems.Sum(i => i.Dish.DishIncrementalPrice * i.Quantity);
            var warningMsg = processor.Errors;
            encodedRecipe = customBentoBox.EncodedRecipe;
            return Json(new { encodedRecipe = encodedRecipe, currentItems = new { mainCourses = mainCourses, sideDishes = sideDishes, others = others }, warningMsg = warningMsg, unitPrice = unitPrice });
        }
        //
        // AJAX: /CustomBentoBox/AddToCurrentBox/
        [HttpPost]
        public ActionResult AddToCurrentBox(string encodedRecipe, int dishId)
        {
            // Retrieve the album from the database
            var addedDish = db.Dishes.Single(i => i.DishId == dishId);

            // Add it to the shopping cart
            var customBentoBox = new CustomBentoBox(encodedRecipe);
            var isAdded = customBentoBox.AddToCustomBentoBox(addedDish);

            var processor = new Processor();
            processor.ProcessRule(customBentoBox);
            if (!processor.CanAddDish)
            {
                // need to remove the current dish!
                customBentoBox.RemoveFromCustomBentoBox(addedDish);
            }
            if (isAdded == false)
            {
                processor.AddDuplicationMessage();
            }
            var mainCourses = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.MainCourse
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             };
            var sideDishes = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.SideDish
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             };
            var others = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.Drink
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             }; 
            var warningMsg = processor.Errors;
            encodedRecipe = customBentoBox.EncodedRecipe;
            var unitPrice = customBentoBox.BentoBox.UnitPrice +customBentoBox.CustomBentoBoxItems.Sum(i => i.Dish.DishIncrementalPrice * i.Quantity);
            return Json(new { 
                encodedRecipe = encodedRecipe, 
                currentItems =  
                    new { 
                        mainCourses =  new { dishes = mainCourses, count = customBentoBox.BentoBox.NumOfEntree } , 
                        sideDishes = new { dishes = sideDishes, count = 1 } ,
                        others = new { dishes = others, count = 1 } }, 
                warningMsg = warningMsg ,
                isValid = isAdded && processor.CanAddDish,
                unitPrice = unitPrice
            });
        }

        //
        // AJAX: /CustomBentoBox/RemoveFromCurrentBox/
        [HttpPost]
        public ActionResult RemoveFromCurrentBox(string encodedRecipe, int dishId)
        {
            // Retrieve the album from the database
            var removedDish = db.Dishes.Single(i => i.DishId == dishId);

            // Add it to the shopping cart
            var customBentoBox = new CustomBentoBox(encodedRecipe);
            customBentoBox.RemoveFromCustomBentoBox(removedDish);
            var processor = new Processor();
            processor.ProcessRule(customBentoBox);
            var mainCourses = from item in customBentoBox.CustomBentoBoxItems
                              where item.Dish.DishTypeId == (int)DishType.MainCourse
                              select new
                              {
                                  dishId = item.DishId,
                                  dishImageUrl = item.Dish.DishImageUrl,
                                  dishName = item.Dish.DishName,
                                  customBentoBoxId = customBentoBox.CustomBentoBoxId
                              };
            var sideDishes = from item in customBentoBox.CustomBentoBoxItems
                             where item.Dish.DishTypeId == (int)DishType.SideDish
                             select new
                             {
                                 dishId = item.DishId,
                                 dishImageUrl = item.Dish.DishImageUrl,
                                 dishName = item.Dish.DishName,
                                 customBentoBoxId = customBentoBox.CustomBentoBoxId
                             };
            var others = from item in customBentoBox.CustomBentoBoxItems
                         where item.Dish.DishTypeId == (int)DishType.Drink
                         select new
                         {
                             dishId = item.DishId,
                             dishImageUrl = item.Dish.DishImageUrl,
                             dishName = item.Dish.DishName,
                             customBentoBoxId = customBentoBox.CustomBentoBoxId
                         };

            var unitPrice = customBentoBox.BentoBox.UnitPrice + customBentoBox.CustomBentoBoxItems.Sum(i => i.Dish.DishIncrementalPrice * i.Quantity);
            var warningMsg = processor.Errors;
            encodedRecipe = customBentoBox.EncodedRecipe;
            return Json(new { encodedRecipe = encodedRecipe, currentItems = new { mainCourses = mainCourses, sideDishes = sideDishes, others = others }, warningMsg = warningMsg, unitPrice = unitPrice });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [HttpPost]
        public ActionResult ResetDeliveryLocationAndTime(string restaurantName)
        {
            LocationHelper.ClearDeliveryLocation(HttpContext);
            ViewData["Reset"] = "1";
            ViewData["RestaurantName"] = restaurantName;
            return PartialView("_LocationSideBar");
        }

        [HttpPost]
        public ActionResult ChangeLocationAndTime(int changeToLocationId, string restaurantName)
        {
            var deliveryLocation = db.University_Delivery.Single(i => i.UniversityDeliveryId == changeToLocationId);
            LocationHelper.SetDeliveryLocation(HttpContext, deliveryLocation);
            ViewData["RestaurantName"] = restaurantName;
            return PartialView("_LocationSideBar");
        }

    }



}
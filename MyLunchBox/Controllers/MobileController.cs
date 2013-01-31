using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using MyLunchBox.Utilities;

namespace MyLunchBox.Controllers
{
    public class MobileController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        //
        // GET: /Mobile/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectUniversity()
        {
            var universities = db.Universities;
            return View(universities);
        }

        public ActionResult ChooseRestaurant(int universityId = 0)
        {
            if (universityId == 0)
            {
                var selectedUniversity = LocationHelper.GetSelectedUniversity(HttpContext);
                if (selectedUniversity != null)
                {
                    universityId = selectedUniversity.UniversityId;
                }
                else
                {
                    return View(new List<Restaurant>());
                }
            }
            var restaurants = db.Restaurants.Where(i => i.UniversityId == universityId);
            return View(restaurants);
        }

        public ActionResult ChooseBentoBox(int restaurantId)
        {
            var restaurant = db.Restaurants.Single(i => i.RestaurantId == restaurantId);
            if (restaurant != null)
            {
                var bentoBoxes = restaurant.BentoBoxes.Where(i => i.BentoBoxStatusId == (int)BentoBoxStatusLevel.Available);
                return View(bentoBoxes);
            }
            return View();
        }

        public ActionResult Create(int bentoBoxId, int customBentoBoxId = 0)
        {
            CustomBentoBox customBentoBox;
            if (customBentoBoxId == 0)
            {
                customBentoBox = new CustomBentoBox()
                {
                    BentoBoxId = bentoBoxId,
                    CustomBentoBoxName = "",
                    BentoBox = db.BentoBoxes.Single(i => i.BentoBoxId == bentoBoxId)
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

        [HttpPost]
        public ActionResult AddToCart(FormCollection values)
        {
            int customBentoBoxId = Convert.ToInt32(values["customBentoBoxId"]);
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
                shoppingCart.AddToCart(customBentoBox);
                return RedirectToAction("Cart");
            }
            return RedirectToAction("Create");
        }
        public ActionResult Cart(int orderId = 0)
        {
            var shoppingCart = ShoppingCartHelper.GetCart(this);
            if (orderId != 0)
            {
                var order = db.Orders.Single(i => i.OrderId == orderId);
                if (order != null)
                {
                    foreach (var orderedItem in order.OrderItems)
                    {
                        var customBentoBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == orderedItem.ItemId);
                        shoppingCart.AddToCart(customBentoBox);
                    }
                }
            }
            db.SaveChanges();
            return View(shoppingCart);
        }

        public ActionResult Checkout()
        {
            try
            {
                var cart = ShoppingCartHelper.GetCart(HttpContext);
                if (cart.ShoppingCartItems.Count() == 0)
                {
                    return RedirectToAction("Edit", "ShoppingCart");
                }
                ViewData["cart"] = cart;
                var deliveryLocation = LocationHelper.GetDeliveryLocation(HttpContext);
                if (deliveryLocation == null)
                {
                    deliveryLocation = db.University_Delivery.First(i => i.UniversityId == 1);
                    LocationHelper.SetDeliveryLocation(HttpContext, deliveryLocation);
                }
                ViewData["deliveryLocation"] = deliveryLocation;
                DateTime deliveryTime;
                if (deliveryLocation.DeliveryTime.Hour < DateTime.Now.Hour)
                {
                    deliveryTime = DateTime.Now.Date.AddDays(1).AddHours(deliveryLocation.DeliveryTime.Hour);
                }
                else
                {
                    deliveryTime = DateTime.Now.Date.AddHours(deliveryLocation.DeliveryTime.Hour);
                }
                ViewData["deliveryTime"] = deliveryTime;
                var currentUserId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
                var totalRewardPoints = 0.0m;
                var pts = db.Rewards.Where(i => i.UserId == currentUserId).Sum(i => (decimal?)i.Amount);
                if (pts.HasValue)
                {
                    totalRewardPoints = pts.Value;
                }
                ViewData["totalRewardPoints"] = Convert.ToInt32(totalRewardPoints);
                var order = new Order();
                var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
                var payer = (MyLunchBoxMembershipUser)(new MyLunchBoxMembershipProvider().GetUser(User.Identity.Name, true));

                order.UserId = userId.Value;
                order.OrderReceivedAt = DateTime.Now;
                order.PayerEmail = User.Identity.Name;
                order.Fee = 0.0m;
                order.Gross = cart.ShoppingCartItems.Sum(i => i.LineItemCost);
                //todo: change after add reword points
                if (cart.CanUserRewardPoint)
                {
                    order.rewardPoints = order.Gross * 100 > totalRewardPoints ? Convert.ToInt32(totalRewardPoints) : Convert.ToInt32(order.Gross * 100);
                }
                else
                {
                    order.rewardPoints = 0;
                }
                order.Savings = order.rewardPoints / 100.0m;
                order.OrderDescription = string.Join(",", cart.ShoppingCartItems.Select(i => i.Description).ToList());
                order.Tax = 0.0m;
                order.PaymentType = PaymentType.NotRequired;

                order.billingFirstName = payer.UserDetails.FirstName;
                order.billingLastName = payer.UserDetails.LastName;
                order.NeedDeliveryInfo = cart.NeedDeliveryInfo;
                if (cart.NeedDeliveryInfo)
                {
                    order.ReceiverFirstName = payer.UserDetails.FirstName;
                    order.ReceiverLastName = payer.UserDetails.LastName;
                    order.ReceiverPhoneNumber = payer.UserDetails.PhoneNumber;
                    order.DeliveryTime = deliveryTime;
                }
                var billingLocation = payer.UserDetails.Location;
                order.billingFirstName = billingLocation != null ? billingLocation.FirstName : "";
                order.billingLastName = billingLocation != null ? billingLocation.LastName : "";
                order.billingAddress1 = billingLocation != null ? billingLocation.Address1 : "";
                order.billingAddress2 = billingLocation != null ? billingLocation.Address2 : "";
                order.billingCity = billingLocation != null ? billingLocation.City : "";
                order.billingState = billingLocation != null ? billingLocation.StateOrProvince : "";
                order.billingZipCode = billingLocation != null ? billingLocation.ZipCode : "";
                return View(order);

            }
            catch
            {
                return HttpNotFound();
            }
        }

        public ActionResult CheckoutComplete()
        {
            return View();
        }
    }
}

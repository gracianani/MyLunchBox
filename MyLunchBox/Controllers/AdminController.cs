using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using System.Data.SqlClient;
using System.Configuration;
using PayPal.PayPalAPIInterfaceService.Model;
using System.Web.Helpers;
using System.Text;
using System.IO;

namespace MyLunchBox.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();

        public ActionResult ManageRestaurants()
        {
            return View(db.Restaurants.ToList());
        }

        public ActionResult CreateRestaurant()
        {
            var universities = db.Universities.Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).ToList()
                                              .Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString() });
            ViewData["Universities"] = universities.ToList();
            ViewData["NewRestaurant"] = "1";
            var restaurant = new Restaurant();
            return View("ManageRestaurantDetails", restaurant);
        }

        [HttpPost]
        public ActionResult CreateRestaurant(FormCollection values)
        {
            var location = new Location() {
                LocationName = values["Location_LocationName"],
                BusinessName = values["Location_LocationName"],
                Address1 = values["Location_Address1"],
                Address2 = values["Location_Address2"],
                City = values["Location_City"],
                StateOrProvince = values["Location_StateOrProvince"],
                ZipCode = values["Location_ZipCode"],
                FirstName = "",
                LastName = "",
                Country = "United States",
                 CountryCode= CountryCodeType.US.ToString()
            };
            db.Locations.AddObject(location);
            DateTime? hoursFrom=null, hoursTo=null, hours2From=null, hours2To=null;
            if(!string.IsNullOrEmpty(values["RestaurantHoursFrom"])) {
                hoursFrom = DateTime.Parse(values["RestaurantHoursFrom"]);
            }
            if(!string.IsNullOrEmpty(values["RestaurantHoursTo"])) {
                hoursTo = DateTime.Parse(values["RestaurantHoursTo"]);
            }
            if(!string.IsNullOrEmpty(values["RestaurantHours2From"])) {
                hours2From = DateTime.Parse(values["RestaurantHours2From"]);
            }
            if(!string.IsNullOrEmpty(values["RestaurantHours2To"])) {
                hours2To = DateTime.Parse(values["RestaurantHours2To"]);
            }
            var restaurant = new Restaurant()
            {
                RestaurantName = values["RestaurantName"],
                RestaurantShortDescription = values["RestaurantShortDescription"],
                RestaurantDetailedDescription = values["RestaurantDetailedDescription"],
                RestaurantHoursFrom = hoursFrom,
                RestaurantHoursTo = hoursTo,
                RestaurantHours2From = hours2From,
                RestaurantHours2To = hours2To,
                UniversityId = int.Parse(values["UniversityId"]),
                RestaurantStatusId = (int)Enum.Parse(typeof(RestaurantStatusLevel), values["RestaurantStatus"]),
                Location = location,
                RestaurantImageUrl = "",
            };
            db.Restaurants.AddObject(restaurant);

            var bowl = new BentoBox()
            {
                BentoBoxName = "1-Entree Plate",
                BentoBoxDescription = "1-Entree Plate",
                BentoBoxTypeId = (int)BentoBoxType.OneEntreePlate,
                BentoBoxStatusId = (int)BentoBoxStatusLevel.Unavailable,
                Restaurant = restaurant,
                BentoBoxImageUrl = "",
                UnitPrice = 5.99m
            };
            var twoEntree = new BentoBox()
            {
                BentoBoxName = "2-Entree Plate",
                BentoBoxDescription = "2-Entree Plate",
                BentoBoxTypeId = (int)BentoBoxType.TwoEntreePlate,
                BentoBoxStatusId = (int)BentoBoxStatusLevel.Unavailable,
                Restaurant = restaurant,
                BentoBoxImageUrl = "",
                UnitPrice = 6.99m
            };
            var threeEntree = new BentoBox()
            {
                BentoBoxName = "3-Entree Plate",
                BentoBoxDescription = "3-Entree Plate",
                BentoBoxTypeId = (int)BentoBoxType.ThreeEntreePlate,
                BentoBoxStatusId = (int)BentoBoxStatusLevel.Unavailable,
                Restaurant = restaurant,
                BentoBoxImageUrl = "",
                UnitPrice = 7.99m
            };
            var noodleAndRice = new BentoBox()
            {
                BentoBoxName = "Noodle and Rice",
                BentoBoxDescription = "Noodle and Rice",
                BentoBoxTypeId = (int)BentoBoxType.NoodleAndRice,
                BentoBoxStatusId = (int)BentoBoxStatusLevel.Unavailable,
                Restaurant = restaurant,
                BentoBoxImageUrl = "",
                UnitPrice = 6.99m
            };
            db.BentoBoxes.AddObject(bowl);
            db.BentoBoxes.AddObject(twoEntree);
            db.BentoBoxes.AddObject(threeEntree);
            db.BentoBoxes.AddObject(noodleAndRice);
            db.SaveChanges();
            return RedirectToAction("ManageRestaurants");
        }

        public ActionResult ManageRestaurantDetails(int restaurantId)
        {
            Restaurant restaurant = db.Restaurants.Single(r => r.RestaurantId == restaurantId);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            var universities = db.Universities.Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).ToList()
                                              .Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString(), Selected = i.Value == restaurant.UniversityId });
            ViewData["Universities"] = universities.ToList();
            return View(restaurant);
        }

        [HttpPost]
        public ActionResult ManageRestaurantDetails(FormCollection values)
        {
            var restaurantId = Convert.ToInt32(values["restaurantId"]);
            var restaurant = db.Restaurants.Single(i => i.RestaurantId == restaurantId);
            restaurant.RestaurantName = values["RestaurantName"];
            restaurant.RestaurantStatusId = (int)Enum.Parse(typeof(RestaurantStatusLevel), values["RestaurantStatus"]);
            restaurant.UniversityId = Convert.ToInt32(values["UniversityId"]);
            DateTime? hoursFrom = null, hoursTo = null, hours2From = null, hours2To = null;
            if (!string.IsNullOrEmpty(values["RestaurantHoursFrom"]))
            {
                hoursFrom = DateTime.Parse(values["RestaurantHoursFrom"]);
            }
            if (!string.IsNullOrEmpty(values["RestaurantHoursTo"]))
            {
                hoursTo = DateTime.Parse(values["RestaurantHoursTo"]);
            }
            if (!string.IsNullOrEmpty(values["RestaurantHours2From"]))
            {
                hours2From = DateTime.Parse(values["RestaurantHours2From"]);
            }
            if (!string.IsNullOrEmpty(values["RestaurantHours2To"]))
            {
                hours2To = DateTime.Parse(values["RestaurantHours2To"]);
            }
            restaurant.RestaurantHoursFrom = hoursFrom;
            restaurant.RestaurantHoursTo = hoursTo;
            restaurant.RestaurantHours2From = hours2From;
            restaurant.RestaurantHours2To = hours2To;
            restaurant.Location.LocationName = values["Location_LocationName"];
            restaurant.Location.Address1 = values["Location_Address1"];
            restaurant.Location.Address2 = values["Location_Address2"];
            restaurant.Location.City = values["Location_City"];
            restaurant.Location.StateOrProvince = values["Location_StateOrProvince"];
            restaurant.Location.ZipCode = values["Location_ZipCode"];
            

            if (!string.IsNullOrEmpty(values["dish.dishId"]))
            {
                var dishIds = values["dish.dishId"].Split(',');
                var dishAvailabilities = values["dish"].Split(',');
                var dishIncrementalPrices = values["dish.DishIncrementalPrice"].Split(',');
                var dishImageUrls = values["dish.DishImageUrl"].Split(',');
                var dishNames = values["dish.DishName"].Split(',');
                for (int i = 0; i < dishIds.Length; i++)
                {
                    var dish = restaurant.Dishes.Single(d => d.DishId == Convert.ToInt32(dishIds[i]));
                    dish.DishStatusId = (int)Enum.Parse(typeof(DishStatusLevel), dishAvailabilities[i]);
                    dish.DishImageUrl = ConfigurationManager.AppSettings["DishImageFolder"] + dishImageUrls[i];
                    dish.DishName = dishNames[i];
                    decimal dishIncrementalPrice;
                    if (decimal.TryParse(dishIncrementalPrices[i], out dishIncrementalPrice))
                    {
                        dish.DishIncrementalPrice = dishIncrementalPrice;
                    }
                }
            }
            if (!string.IsNullOrEmpty(values["bentoBox.BentoBoxId"]))
            {
                var bentoboxIds = values["bentoBox.BentoBoxId"].Split(',');
                var bentoboxNames = values["bentoBox.BentoBoxName"].Split(',');
                var bentoboxAvailabilities = values["bentoBox.BentoBoxStatusId"].Split(',');
                var bentoboxPrices = values["bentoBox.UnitPrice"].Split(',');
                for (int i = 0; i < bentoboxIds.Length; i++)
                {
                    var bentoBox = restaurant.BentoBoxes.Single(b => b.BentoBoxId == Convert.ToInt32(bentoboxIds[i]));
                    bentoBox.BentoBoxStatusId = (int)Enum.Parse(typeof(BentoBoxStatusLevel), bentoboxAvailabilities[i]);
                    bentoBox.UnitPrice = Convert.ToDecimal(bentoboxPrices[i]);
                    bentoBox.BentoBoxName = bentoboxNames[i];
                }
            }
            try
            {
                db.Restaurants.ApplyCurrentValues(restaurant);
                db.SaveChanges();
                ViewBag.UpdateStatus = "Update Successful";
            }
            catch (Exception ex)
            {
                ViewBag.UpdateStatus = ex.Message;
            }
            var universities = db.Universities.Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).ToList()
                                              .Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString(), Selected = i.Value == restaurant.UniversityId });
            ViewData["Universities"] = universities.ToList();
            return RedirectToAction("ManageRestaurantDetails", new {restaurantId = restaurant.RestaurantId});
        }

        [HttpPost]
        public ActionResult AddDish(string dishName, int dishTypeId, string imageName, decimal price, int restaurantId)
        {
            var dish = new Dish()
            {
                DishImageUrl = ConfigurationManager.AppSettings["DishImageFolder"] + imageName,
                IsOnVoting = false,
                RestaurantId = restaurantId,
                ShortDescription = "yummy",
                DetailedDescription = "yummy",
                DishStatusId = (int)DishStatusLevel.Available,
                DishTypeId = dishTypeId,
                DishName = dishName,
                DishIncrementalPrice = price
            };
            db.Dishes.AddObject(dish);
            db.SaveChanges();
            return Json(new { result = "Dish has been added, please hit F5" });
        }

        public ActionResult ManageDeliveryHours()
        {
            
            var universityList = db.Universities.Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).ToList()
                                              .Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString()});
            return View(new UniversityDeliveryViewModel()
            {
                Universities = db.Universities.AsEnumerable(),
                UniversityList = universityList
            });
        }

        public ActionResult AddDeliveryLocation(int universities, string locationName, string address1, string address2, string city, string state, string zipCode, string phoneNumber, string DeliveryTime)
        {
            var location = new Location()
            {
                LocationName = locationName,
                BusinessName = locationName,
                FirstName = locationName,
                LastName = locationName,
                Address1 = address1,
                Address2 = address2,
                City = city,
                StateOrProvince = state,
                ZipCode = zipCode,
                PhoneNumber = phoneNumber,
                Country = "United States",
                CountryCode = "US"
            };
            db.Locations.AddObject(location);
            var deliveryLocation = new University_Delivery()
            {
                UniversityId = universities,
                Location = location,
                DeliveryTime = DateTime.Parse(DeliveryTime)
            };
            db.SaveChanges();
            return RedirectToAction("ManageDeliveryHours"); 
        }

        public ActionResult DeleteDeliveryLocation(int UniversityDeliveryId)
        {
            var deliveryLocation = db.University_Delivery.Single(i => i.UniversityDeliveryId == UniversityDeliveryId);
            db.University_Delivery.DeleteObject(deliveryLocation);
            db.SaveChanges();
            return RedirectToAction("ManageDeliveryHours"); 
        }
        public ActionResult LuckyDraw()
        {
            var rewards = db.Rewards.OrderByDescending(i=>i.RewardCreatedAt);
            var orderFrom = DateTime.Now.Date.AddDays(-(int)DateTime.Now.Date.DayOfWeek - 7);
            var orderTo = DateTime.Now.Date.AddDays(-(int)DateTime.Now.Date.DayOfWeek);

            var luckySpinThisWeek = rewards.Where(i => i.RewardTypeId == (int)RewardType.LuckySpin && i.RewardCreatedAt.CompareTo(orderTo) > 0).AsEnumerable();
            var ordersLastWeek = db.Orders.Where(i => i.DeliveryTime.Value.CompareTo(orderFrom) > 0 && i.DeliveryTime.Value.CompareTo(orderTo) > 0 && luckySpinThisWeek.Count(j=>j.UserId == i.UserId.Value) ==0);
            var universities = db.Universities;
            return View( new LuckySpinViewModel() { Candidates = ordersLastWeek, Universities = universities, Rewards =rewards });
        }
        public ActionResult CreateReward(string rewardCode, string rewardEmail, decimal discount, string rewardDesc)
        {
            var userId = MembershipHelper.GetUserIdByEmail(rewardEmail);
            if (userId.HasValue)
            {
                var reward = new Reward()
                {
                    UserId = userId,
                    RewardDescription = rewardDesc,
                    Amount = discount,
                    RewardCreatedAt = DateTime.Now,
                    RewardTypeId = (int)RewardType.LuckySpin
                };
                db.Rewards.AddObject(reward);
                db.SaveChanges();
                ViewBag.CreateStatus = "Successful";
            }
            else
            {
                ViewBag.CreateStatus = "Error, Can't find userId with given email";
            }
            var orderFrom = DateTime.Now.Date.AddDays(-(int)DateTime.Now.Date.DayOfWeek - 7);
            var orderTo = DateTime.Now.Date.AddDays(-(int)DateTime.Now.Date.DayOfWeek);
            var ordersLastWeek = db.Orders.Where(i => i.DeliveryTime.Value.CompareTo(orderFrom) > 0 && i.DeliveryTime.Value.CompareTo(orderTo) > 0 && string.Compare(i.PayerEmail, rewardEmail) == 0);
            var universities = db.Universities;
            var rewards = db.Rewards.OrderByDescending(i => i.RewardCreatedAt);
            return View("LuckyDraw", new LuckySpinViewModel() { Candidates = ordersLastWeek, Universities = universities, Rewards = rewards });
        }

        public ActionResult ManageVotes()
        {
            var restaurants = db.Restaurants.Select(i => new { Text = i.RestaurantName, Value = i.RestaurantId }).AsEnumerable().Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString() }).ToList();
            var dishes = db.Restaurants.ToDictionary(i => i.RestaurantId, v => v.Dishes.ToList());
            var viewFrom = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
            var viewTo = viewFrom.AddDays(7);
            ViewData["restaurants"] = restaurants;
            ViewData["dishes"] = dishes;
            ViewData["votingStats"] = db.Dishes.ToDictionary(i => i.DishId, v => v.votings.Count(c => viewFrom.CompareTo(c.VotedOn) < 0 && c.VotedOn.CompareTo(viewTo) < 0));
            var dishOrderStats = Dish.GetDishOrderStats(viewFrom, viewTo);
            ViewData["dishOrderStats"] = dishOrderStats;
            ViewData["viewFrom"] = viewFrom.ToString("MM-dd-yyyy");
            ViewData["viewTo"] = viewTo.ToString("MM-dd-yyyy");
            return View();
        }
        [HttpPost]
        public ActionResult ManageVotes(FormCollection values)
        {
            var selectedDishes = new List<string>();
            var dishIds = values["dish"].Split(',').ToList();
            for (int i = 0; i < dishIds.Count; i += 2) 
            {
                var dishId = Convert.ToInt32(dishIds[i]);
                var dish = db.Dishes.Single(j => j.DishId == dishId);
                dish.IsOnVoting = Boolean.Parse(dishIds[i + 1]);
                db.Dishes.ApplyCurrentValues(dish);
            }
            db.SaveChanges();
            return RedirectToAction("ManageVotes");
        }
        public ActionResult ViewAllOrders()
        {
            var orders = db.Orders.AsEnumerable().Where(i => i.DeliveryTime.Value.Date.CompareTo(DateTime.Today) == 0).Where(i => i.OrderItems.Any(j => j.ItemTypeId == (int)ItemType.CustomBentoBox));
            return View(orders);
        }

        public ActionResult ViewOrders(int deliveryLocationId, string deliveryDate, int restaurantId)
        {
            var orders = db.Orders.AsEnumerable()
                    .Where(i => i.DeliveryTime.Value.Date.CompareTo(DateTime.Parse(deliveryDate)) == 0)
                    .Where(i => restaurantId == 0 || i.OrderItems.Any(j => j.Dishes.Any(k => k.RestaurantId == restaurantId)))
                    .Where(i => i.OrderItems.Any(j => j.ItemTypeId == (int)ItemType.CustomBentoBox))
                    .Where(i => i.DeliveryLocationId == deliveryLocationId || deliveryLocationId == 0);
            return View("ViewAllOrders", orders);
        }

        public ActionResult ManageUniversities()
        {
            return View(new UniversitiesViewModel() { Universities = db.Universities } );
        }

        [HttpPost]
        public ActionResult ManageUniversities(FormCollection values)
        {
            var location = new Location()
            {
                LocationName = values["LocationName"],
                BusinessName = values["LocationName"],
                FirstName = values["LocationName"],
                LastName = values["LocationName"],
                Address1 = values["Address1"],
                Address2 = values["Address2"],
                City = values["City"],
                StateOrProvince = values["State"],
                ZipCode = values["ZipCode"],
                PhoneNumber = values["PhoneNumber"],
                Country = "United States",
                CountryCode = "US"
            };
            db.Locations.AddObject(location);
            var universityName = values["UniversityName"];
            var university = new University()
            {
                Location = location,
                UniversityName = universityName
            };
            db.Universities.AddObject(university);
            db.SaveChanges();
            return View(new UniversitiesViewModel() { Universities = db.Universities });
        }

        public ActionResult DeleteUniversity(int universityId)
        {
            var university = db.Universities.Single(i => i.UniversityId == universityId);
            db.Universities.DeleteObject(university);
            db.SaveChanges();
            return RedirectToAction("ManageUniversities"); 
        }

        [HttpPost]
        public ActionResult ViewAllOrders(FormCollection values)
        {
            var dateFrom = values["orderFrom"];
            var dateTo = values["orderTo"];
            var currentDeliveryTimeTo = DateTime.Parse(dateFrom);
            var currentDeliveryTimeFrom  = DateTime.Parse(dateTo);
            var orders = db.Orders.Where(i => i.OrderReceivedAt.CompareTo(currentDeliveryTimeTo) < 0 && i.OrderReceivedAt.CompareTo(currentDeliveryTimeFrom) > 0);
            return View(orders);
        }

        [HttpPost]
        public ActionResult ExportOrders(int deliveryLocationId, string deliveryDate, int restaurantId)
        {
            var fileName = string.Format("Orders-{0}-{1}.csv", restaurantId, deliveryDate);
            var orders = db.Orders.AsEnumerable()
                    .Where(i => i.DeliveryTime.Value.Date.CompareTo(DateTime.Parse(deliveryDate)) == 0)
                    .Where(i => restaurantId == 0 || i.OrderItems.Any(j => j.Dishes.Any(k => k.RestaurantId == restaurantId)))
                    .Where(i => i.OrderItems.Any(j => j.ItemTypeId == (int)ItemType.CustomBentoBox))
                    .Where(i => i.DeliveryLocationId == deliveryLocationId || deliveryLocationId == 0);
            var stream = new FileStream( ConfigurationManager.AppSettings["LunchboxesExportCSVFolder"] + fileName, FileMode.OpenOrCreate);
            var writer = new StreamWriter(stream);
            stream.Position = 0;
            writer.WriteLine("OrderId \t Details \t\t Receiver \t\t Delivery Info \t Price");
            foreach (var order in orders)
            {
                writer.Write( order.OrderId + '\t' ); 
                foreach (var orderItem in order.OrderItems.Where(i => i.ItemTypeId == (int)ItemType.CustomBentoBox))
                {
                    writer.Write(orderItem.CustomBentoBoxItems.First().CustomBentoBox.BentoBox.Restaurant.RestaurantName + '\t');
                    writer.Write(orderItem.CustomBentoBoxItems.First().CustomBentoBox.BentoBox.BentoBoxName + '\t');
                    writer.Write( string.Join(",", orderItem.CustomBentoBoxItems.Select( i=> string.Format("{1} * {0}", i.Dish.DishName, i.Quantity) ).ToList() ) + '\t');
                }
                writer.Write( order.PayerEmail + "\t" );
                writer.Write( order.DeliveryLocation.LocationName + "\t" );
                writer.Write( order.DeliveryTime + "\t" );
                writer.Write( order.FinalAmount + "\t" );
                writer.Write( order.PaymentStatus + writer.NewLine );
            }
            writer.Flush();
            stream.Flush();
            stream.Dispose();
            return Json(new { result = "true", message = string.Format( "CSV file {1} has been downloaded to {0}", ConfigurationManager.AppSettings["LunchboxesExportCSVFolder"] , fileName) });
        }
    }
}

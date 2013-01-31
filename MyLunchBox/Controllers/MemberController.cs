using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using System.Web.Security;
using MyLunchBox.Utilities;

namespace MyLunchBox.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private int CurrentUserId
        {
            get
            {
                return MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name).Value;
            }
        }
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        //
        // GET: /Member/

        public ActionResult MyLunchBoxes()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Account;
            var orders =  db.Orders.Where(i => i.UserId == CurrentUserId).OrderByDescending(i=>i.OrderReceivedAt);
            return View(orders);
        }


        public ActionResult MyPayments()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Account;
            return View();
        }

        public ActionResult MyRewards()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Account;
            var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
            var rewards = db.Rewards.Where(i=>i.UserId == userId ).OrderByDescending(i=>i.RewardCreatedAt);
            var rewardCardTypes = db.RewardCards.Select(i => new { i.RewardCardId, i.RewardDescription }).ToList().Select(i => new SelectListItem() { Text = i.RewardDescription, Value = i.RewardCardId.ToString() }).ToList();
            ViewData["rewardCardTypes"] = rewardCardTypes;
            return View(rewards);
        }

        [HttpPost]
        public ActionResult MyRewards(FormCollection values)
        {
            var rewardCardId = Convert.ToInt32(values["rewardCardTypes"]); 
            var cart = ShoppingCartHelper.GetCart(HttpContext);
            var rewardCard = db.RewardCards.Single(i=>i.RewardCardId == rewardCardId);
            cart.AddToCart(rewardCard);
            db.SaveChanges();
            return RedirectToAction("Edit", "ShoppingCart");
        }
        public ActionResult MyVotes()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Account;
            var myUniversity = LocationHelper.GetSelectedUniversityId(HttpContext);
            var restaurants = db.Restaurants.Where(i=>myUniversity.HasValue ? i.UniversityId == myUniversity.Value : true).Select(i => new { Text = i.RestaurantName, Value = i.RestaurantId }).AsEnumerable().Select(i => new SelectListItem() { Text = i.Text, Value = i.Value.ToString() }).ToList();
            var dishes = db.Restaurants.ToDictionary(i=>i.RestaurantId, v=> v.Dishes.ToList());
            var votes = new Dictionary<int, List<Voting>>();
            var votedOn = new Dictionary<int, DateTime>();
            var deadlineStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            var deadlineEnd = deadlineStart.AddDays(7);
            foreach (var restaurantId in dishes.Keys)
            {
                votes.Add(restaurantId, 
                    db.Votings.Where(i => i.Dish.RestaurantId == restaurantId && i.VotedOn.CompareTo(deadlineStart) > 0 && i.VotedOn.CompareTo(deadlineEnd) < 0 && i.VotedBy == CurrentUserId)
                    .ToList());
                if (votes[restaurantId].Count() > 0)
                {
                    votedOn.Add(restaurantId, votes[restaurantId].First().VotedOn);
                }
            }
            ViewData["restaurants"] = restaurants;
            ViewData["dishes"] = dishes;
            ViewData["votes"] = votes;
            ViewData["votedOn"] = votedOn;
            var totalVotings = db.Votings.Count(c => deadlineStart.CompareTo(c.VotedOn) < 0 && c.VotedOn.CompareTo(deadlineEnd) < 0 && c.Dish.IsOnVoting == true);
            
            ViewData["votingStats"] = db.Dishes.ToDictionary(i => i.DishId, v => string.Format("{0:0.##}", 
                totalVotings == 0 ? 0 : (decimal)v.votings.Count(c => deadlineStart.CompareTo(c.VotedOn) < 0 && c.VotedOn.CompareTo(deadlineEnd) < 0 && c.Dish.IsOnVoting == true) / totalVotings * 100));

            ViewData["orderStats"] = Dish.GetDishOrderStats(deadlineStart, deadlineEnd).OrderByDescending(i=>i.Value).ToDictionary(i=>i.Key, j=>j.Value); 
            return View();
        }

        [HttpPost]
        public ActionResult MyVotes(FormCollection values)
        {
            var selectedDishes = new List<string>();

            for (int i = 0; i < values.Count; i++)
            {
                var dishId = Convert.ToInt32(values[i]);
                var dish = db.Dishes.Single(j => j.DishId == dishId);
                var voting = new Voting() { DishId = dishId, VotedOn = DateTime.Now , VotedBy= CurrentUserId};
                db.Votings.AddObject(voting);
            }

            var reward = new Reward()
            {
                RewardCreatedAt = DateTime.Now,
                RewardDescription = "Voting",
                Amount = 30,
                UserId = CurrentUserId,
                RewardTypeId = (int)RewardType.Voting
            };
            db.Rewards.AddObject(reward);

            db.SaveChanges();
            return RedirectToAction("MyVotes");
        }
        public ActionResult MembershipDetails()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Account;
            var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
            var userDetails = db.UserDetails.Single(i => i.UserId == userId);
                    
            var universities = db.Universities
                                    .Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).AsEnumerable()
                                    .Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString(), Selected = (userDetails.UniversityId.HasValue ? x.Value == userDetails.UniversityId.Value : false )}).ToList();
            var deliveryLocations = db.University_Delivery
                                    .Where(i => userDetails.UniversityId.HasValue ? i.UniversityId == userDetails.UniversityId : false)
                                    .Select(i => new { i.UniversityDeliveryId, i.Location.BusinessName, i.DeliveryTime }).AsEnumerable()
                                    .Select(i => new SelectListItem { Text = i.BusinessName + " " + i.DeliveryTime.ToString("hh:mm tt"), Value = i.UniversityDeliveryId.ToString(), Selected = userDetails.UniversityDeliveryId.HasValue? userDetails.UniversityDeliveryId.Value == i.UniversityDeliveryId : false }).ToList();

            var detailModel = new UserDetailsViewModel
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                PhoneNumber = userDetails.PhoneNumber,
                LocationFirstName = userDetails.Location != null ? userDetails.Location.FirstName : "",
                LocationLastName = userDetails.Location != null ? userDetails.Location.LastName : "",
                Address1 = userDetails.Location != null ? userDetails.Location.Address1 : "",
                Address2 = userDetails.Location != null ? userDetails.Location.Address2 : "",
                City = userDetails.Location != null ? userDetails.Location.City : "",
                State = userDetails.Location != null ? userDetails.Location.StateOrProvince : "",
                ZipCode = userDetails.Location != null ? userDetails.Location.ZipCode : "",
                Universities = universities,
                States = MyLunchBox.Models.StatesRepository.Instance.StatesListWithSelectedItem(userDetails.Location != null ? userDetails.Location.StateOrProvince : ""),
                DeliveryLocations = deliveryLocations,
                UniversityId = userDetails.UniversityId.HasValue ? userDetails.UniversityId.Value : -1,
                UniversityDeliveryId = userDetails.UniversityDeliveryId.HasValue ? userDetails.UniversityDeliveryId.Value : -1
            };
            return View( detailModel);
        }

        [HttpPost]
        public ActionResult MembershipDetails(FormCollection values)
        {
            var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
            var userDetails = db.UserDetails.Single(i=>i.UserId == userId);
            userDetails.FirstName = values["firstName"];
            userDetails.LastName = values["lastName"];
            userDetails.PhoneNumber = values["phoneNumber"];
            userDetails.UniversityId = Convert.ToInt32(values["UniversityId"]);
            userDetails.UniversityDeliveryId = Convert.ToInt32(values["UniversityDeliveryId"]);
            if (userDetails.Location == null)
            {
                var location = new Location()
                {
                    LocationName = values["LocationFirstName"] + values["LocationLastName"],
                    FirstName = values["LocationFirstName"],
                    LastName = values["LocationLastName"],
                    Address1 = values["Address1"],
                    Address2 = values["Address2"],
                    City = values["City"],
                    StateOrProvince = values["State"],
                    ZipCode = values["ZipCode"],
                    CountryCode = "US",
                    Country = "United States"
                };
                db.Locations.AddObject(location);
                db.SaveChanges();
                userDetails.LocationId = location.LocationId;
            }
            else
            {
                userDetails.Location.FirstName = values["LocationFirstName"];
                userDetails.Location.LastName = values["LocationLastName"];
                userDetails.Location.Address1 = values["Address1"];
                userDetails.Location.Address2 = values["Address2"];
                userDetails.Location.City = values["City"];
                userDetails.Location.StateOrProvince = values["State"];
                userDetails.Location.ZipCode = values["ZipCode"];
            }
            try
            {
                db.UserDetails.ApplyCurrentValues(userDetails);
                db.SaveChanges();
                ViewBag.UpdateStatus = "Successful";
            }
            catch (Exception ex)
            {
                ViewBag.UpdateStatus = ex.Message;
            }
            var universities = db.Universities
                                    .Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).AsEnumerable()
                                    .Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString(), Selected = (userDetails.UniversityId.HasValue ? userDetails.UniversityId.Value == x.Value : false)}).ToList();
            var deliveryLocations = db.University_Delivery
                                    .Where(i => userDetails.UniversityId.HasValue ? i.UniversityId == userDetails.UniversityId : false)
                                    .Select(i => new { i.UniversityDeliveryId, i.Location.BusinessName, i.DeliveryTime }).AsEnumerable()
                                    .Select(i => new SelectListItem { Text = i.BusinessName + " " + i.DeliveryTime.ToString("hh:mm tt"), Value = i.UniversityDeliveryId.ToString(),
                                        Selected = (userDetails.UniversityDeliveryId.HasValue? i.UniversityDeliveryId== userDetails.UniversityDeliveryId.Value : false )}).ToList();
            var detailModel = new UserDetailsViewModel
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                PhoneNumber = userDetails.PhoneNumber,
                LocationFirstName = userDetails.Location != null ? userDetails.Location.FirstName : "",
                LocationLastName = userDetails.Location != null ? userDetails.Location.LastName : "",
                Address1 = userDetails.Location != null ? userDetails.Location.Address1 : "",
                Address2 = userDetails.Location != null ? userDetails.Location.Address2 : "",
                City = userDetails.Location != null ? userDetails.Location.City : "",
                State = userDetails.Location != null ? userDetails.Location.StateOrProvince : "",
                ZipCode = userDetails.Location != null ? userDetails.Location.ZipCode : "",
                Universities = universities,
                States = MyLunchBox.Models.StatesRepository.Instance.StatesListWithSelectedItem(userDetails.Location != null ? userDetails.Location.StateOrProvince : ""),
                DeliveryLocations = deliveryLocations,
                UniversityId = userDetails.UniversityId.HasValue ? userDetails.UniversityId.Value : -1,
                UniversityDeliveryId = userDetails.UniversityDeliveryId.HasValue ? userDetails.UniversityDeliveryId.Value : -1
            };
            return View(detailModel);
        }

        public ActionResult GetDeliveryLocations(int universityId)
        {
            var db = new MyLunchBoxDevelopmentEntities();
            var deliveries = db.University_Delivery.Where(i => i.UniversityId == universityId).Select(i => new { i.UniversityDeliveryId, i.Location.BusinessName, i.DeliveryTime }).AsEnumerable()
                .Select(i => new SelectListItem() { Text = i.BusinessName + " " + i.DeliveryTime.ToString("hh:mm tt"), Value = i.UniversityDeliveryId.ToString() });
            return PartialView(deliveries);
        }
    }
}

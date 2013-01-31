using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;

namespace MyLunchBox.Controllers
{
    public class LocationController : Controller
    {
        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();


        [HttpPost]
        public ActionResult CurrentLocation(int universityId = 0)
        {
            string universityName;
            if (universityId != 0)
            {
                var university = db.Universities.Single(i => i.UniversityId == universityId);
                LocationHelper.SetSelectedUniversity( HttpContext, university);
                universityName = university.UniversityName;
                LocationHelper.ClearDeliveryLocation(HttpContext);
                ShoppingCartHelper.EmptyCart(HttpContext);
            }
            else
            {
                universityName = "";
                LocationHelper.ClearSelectedUniversity(HttpContext);
            }
            var newModel = GetMyLocationViewModel();
            newModel.SelectedUniversity = universityName;
            return Json( newModel);
        }
     
        public ActionResult ChangeLocationAndTime( int changeToLocationId )
        {
            var deliveryLocation = db.University_Delivery.Single(i => i.UniversityDeliveryId == changeToLocationId);
            LocationHelper.SetDeliveryLocation(HttpContext, deliveryLocation);
            return LocationSideBar();
        }
        public ActionResult ResetUniversity()
        {
            LocationHelper.ClearSelectedUniversity(HttpContext);
            var newModel = GetMyLocationViewModel();
            newModel.SelectedUniversity = "";
            return PartialView("LocationSideBar", newModel);

        }
        public ActionResult ResetDeliveryLocationAndTime()
        {
            LocationHelper.ClearDeliveryLocation(HttpContext);
            var newModel = GetMyLocationViewModel();
            newModel.SelectedLocation = null;
            return PartialView("LocationSideBar", newModel);
        }
        public ActionResult ChangeUniversity(int changeToUniversityId)
        {
            var university = db.Universities.Single(i => i.UniversityId == changeToUniversityId);
            LocationHelper.SetSelectedUniversity(HttpContext, university);
            return LocationSideBar();
        }

        public ActionResult LocationSideBar()
        {
            var myLocation = GetMyLocationViewModel();
            return PartialView("LocationSideBar", myLocation);
        }

        private MyLocationViewModel GetMyLocationViewModel()
        {
            var university = LocationHelper.GetSelectedUniversity(HttpContext);
            var universities = db.Universities.Select(i => new { Text = i.UniversityName, Value = i.UniversityId }).AsEnumerable().Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString() }).ToList();

            var deliveryLocation = LocationHelper.GetDeliveryLocation(HttpContext);

            var locations = new List<SelectListItem>();
            if (university != null)
            {
                locations = db.University_Delivery
                                  .Where(i=>i.UniversityId == university.UniversityId)
                                  .Select(i => new { Text = i.Location.BusinessName, Value = i.UniversityDeliveryId, Time = i.DeliveryTime }).AsEnumerable().Select(x => new SelectListItem { Text = x.Text + " " + x.Time.ToString("hh:mm tt"), Value = x.Value.ToString() }).ToList();
            }

            var myLocation = new MyLocationViewModel()
            {
                Locations = locations,
                SelectedLocation = deliveryLocation == null ? "" : deliveryLocation.Location.BusinessName + " " + deliveryLocation.DeliveryTime.ToString("hh:mm tt"),
                Universities = universities,
                SelectedUniversity = university != null ? university.UniversityName : "",
                SelectedRestaurant = ""
            };
            return myLocation;
        }
    }
}

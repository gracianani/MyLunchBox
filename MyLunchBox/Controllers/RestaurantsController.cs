using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;

namespace MyLunchBox.Controllers
{ 
    public class RestaurantsController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();

        //
        // GET: /Restaurants/

        public ViewResult Index()
        {
            return View(db.Restaurants.ToList());
        }

        //
        // GET: /Restaurants/Details/5

        public ViewResult Details(int id)
        {
            Restaurant restaurant = db.Restaurants.Single(r => r.RestaurantId == id);
            return View(restaurant);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
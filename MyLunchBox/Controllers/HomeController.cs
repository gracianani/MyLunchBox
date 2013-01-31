using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;

namespace MyLunchBox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Home;
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}

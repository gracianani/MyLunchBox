using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;

namespace MyLunchBox.Controllers
{
    public class VotingController : Controller
    {
        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        public ActionResult WeeklyVoting()
        {
            return PartialView(db.Dishes);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;

namespace MyLunchBox.Controllers
{
    public class RewardController : Controller
    {
        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        public ActionResult LuckySpinHome()
        {
            var rewardFrom = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
            var luckySpins = db.Rewards.Where(i => i.RewardTypeId == (int)RewardType.LuckySpin && i.RewardCreatedAt.CompareTo(rewardFrom) > 0 )
                                .OrderByDescending( i=>i.RewardCreatedAt );
            return PartialView(luckySpins);
        }
    }
}

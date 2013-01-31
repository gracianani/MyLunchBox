using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using System.Text;
using System.Net;
using System.IO;

namespace MyLunchBox.Controllers
{ 
    public class ShoppingCartController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();

        // reorder
        public ActionResult Edit(int orderId = 0)
        {
            SiteMenuHelper.Instance.CurrentSiteMenu = SiteMenu.Cart;
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

        [HttpPost]
        public ActionResult RemoveItem(int itemId, int itemTypeId)
        {
            ShoppingCart shoppingCart = ShoppingCartHelper.GetCart(this);
            shoppingCart.RemoveFromCart(itemId, itemTypeId);
            db.SaveChanges();
            return Json(new { gross = shoppingCart.Gross });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult ChangeQuantity(int itemId, int itemTypeId, int quantity)
        {
            var shoppingCart = ShoppingCartHelper.GetCart(this);
            shoppingCart.ChangeQuantity(itemId, itemTypeId, quantity);
            db.SaveChanges();
            return Json(new {gross = shoppingCart.Gross});

        }
    }
}
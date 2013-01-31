using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MyLunchBox.Models
{
    public partial class ShoppingCart
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        public void Swap(int swapWithItemId, int swapToItemId)
        {

        }

        public int? CurrentUserId
        {
            get {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return MembershipHelper.GetUserIdByEmail(HttpContext.Current.User.Identity.Name);
                }
                return null;
            }
        }

        public void AddToCart(IItem item)
        {

            var count = ShoppingCartItems.Count(i => i.ItemId == item.ItemId && i.ItemTypeId == item.ItemTypeId);
            
            if (count == 0)
            {
                var dbShoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = _ShoppingCartId,
                    ItemId = item.ItemId,
                    ItemTypeId = item.ItemTypeId,
                    Quantity = 1,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    UnitPrice = item.UnitPrice
                };
                db.ShoppingCartItems.AddObject(dbShoppingCartItem);
                db.SaveChanges();
                var shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartItemId = dbShoppingCartItem.ShoppingCartItemId,
                    ShoppingCartId = _ShoppingCartId,
                    ItemId = item.ItemId,
                    ItemTypeId = item.ItemTypeId,
                    Quantity = 1,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    UnitPrice = item.UnitPrice
                };
                ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                var shoppingCartItem = ShoppingCartItems.Single(i => i.ItemId == item.ItemId && i.ItemTypeId == item.ItemTypeId);
            }
            
        }

        public int RemoveFromCart(int itemId, int itemTypeId)
        {
            // Get the cart
            var dbShoppingCartItem = db.ShoppingCartItems.Single(i=>i.ShoppingCartId == _ShoppingCartId && i.ItemId == itemId);
            int itemCount = 0;
            if (dbShoppingCartItem != null)
            {
                db.ShoppingCartItems.DeleteObject(dbShoppingCartItem);
                db.SaveChanges();
            }
            return itemCount;
        }

        public void ChangeQuantity(int itemId, int itemTypeId, int quantity)
        {
            var dbShoppingCartItem = ShoppingCartItems.Single(i => i.ItemId == itemId && i.ItemTypeId == itemTypeId);
            
            if (dbShoppingCartItem != null)
            {
                dbShoppingCartItem.Quantity = quantity;

                var shoppingCartItem = db.ShoppingCartItems.Single(i => i.ItemId == itemId && i.ShoppingCartItemId == dbShoppingCartItem.ShoppingCartItemId);
                shoppingCartItem.Quantity = quantity;
                db.ShoppingCartItems.ApplyCurrentValues(shoppingCartItem);
                db.SaveChanges();
            }
        }

        public decimal Gross
        {
            get
            {
                decimal gross = 0.0m;
                foreach (var shoppingCartItem in ShoppingCartItems)
                {
                    gross += shoppingCartItem.LineItemCost;
                }
                return gross;
            }
        }

        public bool CanUserRewardPoint
        {
            get
            {
                if (ShoppingCartItems.Count(i => i.ItemTypeId == (int)ItemType.MembershipCard) > 0)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsRewardPointEnough
        {
            get
            {
                if (CurrentUserId.HasValue)
                {
                    var rewardPts = db.Rewards.Where(i => i.UserId == CurrentUserId.Value).Sum(i => (decimal?)i.Amount);
                    if (rewardPts > Gross * 100)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool NeedDeliveryInfo
        {
            get
            {
                if (ShoppingCartItems.All(i => i.ItemTypeId == (int)ItemType.MembershipCard) )
                {
                    return false;
                }
                return true;
            }
        }

        public bool CanUseCash
        {
            get
            {
                if (ShoppingCartItems.Any(i => i.ItemTypeId == (int)ItemType.MembershipCard))
                {
                    return false;
                }
                return true;
            }
        }

        public static bool SystemCanUseCreditCard
        {
            get
            {
                if (ConfigurationManager.AppSettings["CanUseCreditCard"] == "1")
                {
                    return true;
                }
                return false;
            }
        }

        public static bool SystemCanUseCash
        {
            get
            {
                if (ConfigurationManager.AppSettings["CanUseCash"] == "1")
                {
                    return true;
                }
                return false;
            }
        }

        public static bool SystemCanUseRewardCard
        {
            get
            {
                if (ConfigurationManager.AppSettings["CanUseRewardCard"] == "1")
                {
                    return true;
                }
                return false;
            }
        }
        

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;

namespace MyLunchBox.Models
{
    public partial class ShoppingCartItem
    {
        public string Description
        {
            get
            {
                if (_ItemId != 0 && ItemTypeId == (int)ItemType.CustomBentoBox)
                {
                    MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                    var customBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == _ItemId);
                    var dishNames = customBox.CustomBentoBoxItems.Select(i => i.Dish.DishName);
                    return customBox.BentoBox.BentoBoxDescription + ": " + string.Join(",", dishNames.ToList());
                }
                else if (_ItemId != 0 && ItemTypeId == (int)ItemType.MembershipCard)
                {
                    MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                    var rewardCard = db.RewardCards.Single(i => i.RewardCardId == _ItemId);
                    return rewardCard.ItemDescription;
                }
                return "";
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get
            {
                if (_unitPrice == 0)
                {
                    if (_ItemId != 0 && ItemTypeId == (int)ItemType.CustomBentoBox)
                    {
                        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                        var customBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == _ItemId);
                        _unitPrice = customBox.UnitPrice ;
                    }
                    else if (_ItemId != 0 && ItemTypeId == (int)ItemType.MembershipCard)
                    {
                        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                        var rewardCard = db.RewardCards.Single(i => i.RewardCardId == _ItemId);
                        _unitPrice = rewardCard.UnitPrice;
                    }
                    else
                    {
                        _unitPrice = 0.0m;
                    }
                }
                return _unitPrice; 
            }
            set
            {
                _unitPrice = value;
            }
        }

        public decimal LineItemCost
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }

        public CustomBentoBox CustomBentoBox
        {
            get
            {
                if (_ItemId != 0 && ItemTypeId == (int)ItemType.CustomBentoBox)
                {
                    MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                    var customBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == _ItemId);
                    return customBox;
                }
                return null;
            }
        }

        private string RenderCustomBentoBox(CustomBentoBox customBentoBox)
        {
            var sb = new StringBuilder();
            var dishes = customBentoBox.CustomBentoBoxItems.Select(i=>i.Dish);
            sb.Append("<ul>");
            foreach (var dish in dishes)
            {
                sb.Append(string.Format("<li><img src='{0}'></li>", dish.DishImageUrl));
            }
            sb.Append("</ul");
            return sb.ToString();
        }
    }
}
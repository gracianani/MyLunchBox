using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public partial class OrderItem
    {
        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        public IItem Item
        {
            get
            {
                if (ItemTypeId == (int)ItemType.CustomBentoBox)
                {
                    return db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == ItemId);
                }
                else if (ItemTypeId == (int)ItemType.MembershipCard)
                {
                    return db.RewardCards.Single(i => i.RewardCardId == ItemId);
                }
                return null;
            }
        }
        public List<Dish> Dishes
        {
            get
            {
                var customBentoBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == ItemId);
                if (customBentoBox != null)
                {
                    return customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).ToList();
                }
                return new List<Dish>();
            }
        }
        public List<CustomBentoBoxItem> CustomBentoBoxItems
        {
            get
            {
                var customBentoBox = db.CustomBentoBoxes.Single(i => i.CustomBentoBoxId == ItemId);
                if (customBentoBox != null)
                {
                    return customBentoBox.CustomBentoBoxItems.ToList();
                }
                return new List<CustomBentoBoxItem>();
            }
        }
        public string EditUrl
        {
            get
            {
                if (this.ItemTypeId == (int)ItemType.CustomBentoBox)
                {
                    return string.Format("/CustomBentoBox/Create?CustomBentoBoxId={0}", ItemId);
                }
                return "";
            }
        }
    }
}
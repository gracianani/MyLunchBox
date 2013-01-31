using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public class CustomBentoBoxViewModel
    {
        public CustomBentoBox CurrentCustomBentoBox { get; set; }
        public BentoBoxType SelectedBentoBoxType { get; set; }
        public string RestaurantName { get; set; }
        public Dictionary<DishType, List<Dish>> AvailableDishes { get; set; }
    }
}
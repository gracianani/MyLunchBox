using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public class MyLocationViewModel 
    {
        public string SelectedLocation {get;set;}
        public List<SelectListItem> Locations { get; set; }
        public string SelectedUniversity { get; set; }
        public List<SelectListItem> Universities { get; set; }
        public string SelectedRestaurant { get; set; }
    }
}

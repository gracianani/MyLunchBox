using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyLunchBox.Models
{
    public class LuckySpinViewModel
    {
        public IEnumerable<Order> Candidates { get; set; }
        public IEnumerable<Reward> Rewards { get; set; }
        public IEnumerable<University> Universities { get; set; }
    }

    public class UniversityDeliveryViewModel
    {
        public IEnumerable<SelectListItem> UniversityList { get; set; }
        public IEnumerable<University> Universities { get; set; }
        [Required]
        [RegularExpression(@"\d{2,2}:\d{2,2}", ErrorMessage = "Please put a valid time. example: 17:00")]
        public string DeliveryTime{ get; set;}
        [Required]
        public string LocationName { get; set; }
        [Required]
        public string Address1 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
    }

    public class UniversitiesViewModel
    {
        public IEnumerable<University> Universities { get; set; }
        [Required]
        public string UniversityName { get; set; }
        [Required]
        public string LocationName { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
    }
}
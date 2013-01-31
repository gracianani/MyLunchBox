using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyLunchBox.Models
{
    public class UserDetailsViewModel
    {
        public string LocationFirstName { get; set; }
        public string LocationLastName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", 
            ErrorMessage="Please put a valid US Phone Number")]
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage="Please put a valid US Zipcode")]
        public string ZipCode { get; set; }
        public List<SelectListItem> Universities { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> DeliveryLocations { get; set; }
        public int UniversityId { get; set; }
        public int UniversityDeliveryId { get; set; }
    }
}


using System.Data.Objects;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace MyLunchBox.Models
{
    [MetadataType(typeof(Restaurant_Validation))]
    public partial class Restaurant
    {
        
        public RestaurantStatusLevel RestaurantStatus
        {
            get
            {
                return (RestaurantStatusLevel) Enum.ToObject( typeof(RestaurantStatusLevel), _RestaurantStatusId);
            }
        }

        public string RestaurantHoursString
        {
            get
            {
                return string.Format("{0:hh:mm} - {1:hh:mm}", RestaurantHoursFrom, RestaurantHoursTo);
            }
        }

        public List<SelectListItem> RestaurantStatusLevels {
            get {
                string[] names = Enum.GetNames(typeof(MyLunchBox.Models.RestaurantStatusLevel));
                var values =(MyLunchBox.Models.RestaurantStatusLevel[]) Enum.GetValues(typeof(MyLunchBox.Models.RestaurantStatusLevel));
                var selectList = new List<SelectListItem>();
                for(int i = 0;i < names.Length; i++) {
                    selectList.Add(new SelectListItem() { Text = names[i], Value = values[i].ToString(), Selected = RestaurantStatus == values[i] } );
                }
                return selectList;
            }
        }

        [Required(ErrorMessage = "*")]
        public string Location_LocationName
        {
            get
            {
                if (Location != null)
                {
                    return Location.LocationName;
                }
                return "";
            }
            set
            {
                Location_LocationName = value;
            }
        }

        [Required(ErrorMessage = "*")]
        public string Location_Address1
        {
            get
            {
                if (Location != null)
                {
                    return Location.Address1;
                }
                return "";
            }
            set
            {
                Location_Address1 = value;
            }
        }


        [Required(ErrorMessage = "*")]
        public string Location_City
        {
            get
            {
                if (Location != null)
                {
                    return Location.City;
                }
                return "";
            }
            set
            {
                Location_City = value;
            }
        }

        [Required(ErrorMessage = "*")]
        public string Location_StateOrProvince
        {
            get
            {
                if (Location != null)
                {
                    return Location.StateOrProvince;
                }
                return "";
            }
            set
            {
                Location_StateOrProvince = value;
            }
        }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Please put a valid US Zipcode")]
        public string Location_ZipCode
        {
            get
            {
                if (Location != null)
                {
                    return Location.ZipCode;
                }
                return "";
            }
            set
            {
                Location_ZipCode = value;
            }
        }
    }

    [Bind(Include = "RestaurantName")]
    public class Restaurant_Validation
    {
        [Required(ErrorMessage = "*")]
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "*")]
        public string RestaurantShortDescription { get; set; }

        [Required(ErrorMessage = "*")]
        public string RestaurantDetailedDescription { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"\d{2,2}:\d{2,2}", ErrorMessage = "Please put a valid time")]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime RestaurantHoursFrom { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"\d{2,2}:\d{2,2}", ErrorMessage="Please put a valid time")]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime RestaurantHoursTo { get; set; }

        [RegularExpression(@"\d{2,2}:\d{2,2}", ErrorMessage = "Please put a valid time")]
        public DateTime RestaurantHours2From { get; set; }

        [RegularExpression(@"\d{2,2}:\d{2,2}", ErrorMessage = "Please put a valid time")]
        public DateTime RestaurantHours2To { get; set; }
    }
}
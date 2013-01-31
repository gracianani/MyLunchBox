using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyLunchBox.Models
{
    [MetadataType(typeof(Location_Validation))]
    public partial class Location
    {
    }

    [Bind(Include = "Address1")]
    public class Location_Validation
    {
        [Required(ErrorMessage = "*")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "*")]
        public string Address2 { get; set; }

    }
}
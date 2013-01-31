using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MyLunchBox.Models
{
    public class BillingInfo
    {
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public CountryCodeType CountryCode { get; set; }
    }
}
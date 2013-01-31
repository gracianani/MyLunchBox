using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public class ModelClientValidationDeliveryInfoRequiredRule : ModelClientValidationRule
    {
        public ModelClientValidationDeliveryInfoRequiredRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "deliveryinforequired";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public class ModelClientValidationBillingInfoRequiredRule: ModelClientValidationRule
    {
        public ModelClientValidationBillingInfoRequiredRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "billinginforequired";
        }
    }
}
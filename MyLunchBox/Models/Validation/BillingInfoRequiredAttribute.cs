using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Foolproof;

namespace MyLunchBox.Models
{
    public class BillingInfoRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        private const string errorMessage = "*";

        public BillingInfoRequiredAttribute()
            : base(errorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var grossPropertyInfo = validationContext.ObjectType.GetProperty("Gross");
            var savingPropertyInfo = validationContext.ObjectType.GetProperty("Savings");
            var paymentTypePropertyInfo = validationContext.ObjectType.GetProperty("PaymentType");
            decimal gross = Convert.ToDecimal( grossPropertyInfo.GetValue(validationContext.ObjectInstance, null) );
            decimal savings = Convert.ToDecimal(savingPropertyInfo.GetValue(validationContext.ObjectInstance, null));
            int paymentType = Convert.ToInt32(paymentTypePropertyInfo.GetValue(validationContext.ObjectInstance, null));
            // assuming your country property is bound to a string


            if (gross - savings == 0 || paymentType == (int)PaymentType.Cash)
                 return ValidationResult.Success;
                 // assuming postal code not required for all other countries

            return new ValidationResult(errorMessage);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationBillingInfoRequiredRule(FormatErrorMessage(metadata.GetDisplayName())) };
        }

    }
}
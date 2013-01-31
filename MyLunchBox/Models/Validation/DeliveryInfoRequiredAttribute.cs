using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public class DeliveryInfoRequiredAttribute: ValidationAttribute, IClientValidatable
    {
         private const string errorMessage = "*";

         public DeliveryInfoRequiredAttribute()
            : base(errorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var needDeliveryInfoPropertyInfo = validationContext.ObjectType.GetProperty("NeedDeliveryInfo");
            bool needDeliveryInfo = Convert.ToBoolean(needDeliveryInfoPropertyInfo.GetValue(validationContext.ObjectInstance, null));
            if (needDeliveryInfo)
            {
                return new ValidationResult(errorMessage);
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationDeliveryInfoRequiredRule(FormatErrorMessage(metadata.GetDisplayName())) };
        }

    }
}
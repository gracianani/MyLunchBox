using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLunchBox.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Foolproof;

namespace MyLunchBox.Models
{
    [MetadataType(typeof(Order_Validation))]
    public partial class Order 
    {
        public MyLunchBoxMembershipUser Payer
        {
            get
            {
                var member = (MyLunchBoxMembershipUser)(new MyLunchBoxMembershipProvider().GetUser(PayerEmail, true));
                return member;
            }
        }

        public decimal FinalAmount
        {
            get
            {
                return Gross - Savings;
            }
        }

        [BillingInfoRequired]
        public string billingFirstName { get; set; }

        [BillingInfoRequired]
        public string billingLastName { get; set; }

        [BillingInfoRequired]
        public string billingAddress1 { get; set; }

        public string billingAddress2 { get; set; }
        
        [BillingInfoRequired]
        public string billingCity { get; set; }

        public string billingState { get; set; }

        [BillingInfoRequired]
        [RegularExpression(@"\d{5}(-\d{4})?", ErrorMessage = "Please enter a valide credit card number")]
        public string billingZipCode { get; set; }

        [BillingInfoRequired]
        [DataType(DataType.Text)]
        [RegularExpression(@"\d{16}", ErrorMessage = "Please enter a valide credit card number")]
        public string cardNumber { get; set; }

        public int cardExpMonth { get; set; }

        public int cardExpYear { get; set; }

        [BillingInfoRequired]
        public string CSV { get; set; }

        public int rewardPoints { get; set; }

        public PaymentType PaymentType { get; set; }

        public bool NeedDeliveryInfo { get; set; }

        public string ShortDescription
        {
            get
            {
                if (OrderItems.Count > 0)
                {
                    var orderItem = OrderItems.First();
                    if (orderItem.ItemTypeId == (int)ItemType.CustomBentoBox)
                    {
                        var db = new  MyLunchBoxDevelopmentEntities();
                        var customBox = db.CustomBentoBoxes.Single(i=>i.CustomBentoBoxId == orderItem.ItemId);
                        return customBox.BentoBox.Restaurant.RestaurantName;
                    }
                    else if (orderItem.ItemTypeId == (int)ItemType.MembershipCard)
                    {
                        var db = new MyLunchBoxDevelopmentEntities();
                        var rewardCard = db.RewardCards.Single(i => i.RewardCardId == orderItem.ItemId);
                        return rewardCard.RewardDescription;
                    }
                }
                return "";
            }
        }
    }

    public enum PaymentType
    {
        CreditCard = 1,
        Cash = 2,
        NotRequired = 3
    }

    [Bind(Include="ReceiverFirstName")]
    public class Order_Validation
    {
        [DeliveryInfoRequired(ErrorMessage = "*")]
        public string ReceiverFirstName { get; set; }

        [DeliveryInfoRequired(ErrorMessage = "*")]
        public string ReceiverLastName { get; set; }

        [DeliveryInfoRequired(ErrorMessage = "*")]
        [RegularExpression(@"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", ErrorMessage="Please enter a valid US phone number")]
        public string ReceiverPhoneNumber { get; set; }
    }
}
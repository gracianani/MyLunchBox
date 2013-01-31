using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLunchBox.Models;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;
using MyLunchBox.Utilities;

namespace MyLunchBox.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();

        public ActionResult AddressAndPayment()
        {
            try
            {
                var cart = ShoppingCartHelper.GetCart(HttpContext);
                if (cart.ShoppingCartItems.Count() == 0)
                {
                    return RedirectToAction("Edit", "ShoppingCart");
                }
                ViewData["cart"] = cart;
                var deliveryLocation = LocationHelper.GetDeliveryLocation(HttpContext);
                if (deliveryLocation == null)
                {
                    deliveryLocation = db.University_Delivery.First(i => i.UniversityId == 1);
                    LocationHelper.SetDeliveryLocation(HttpContext, deliveryLocation);
                }
                ViewData["deliveryLocation"] = deliveryLocation;
                DateTime deliveryTime;
                if (deliveryLocation.DeliveryTime.Hour < DateTime.Now.Hour)
                {
                    deliveryTime = DateTime.Now.Date.AddDays(1).AddHours(deliveryLocation.DeliveryTime.Hour);
                }
                else
                {
                    deliveryTime = DateTime.Now.Date.AddHours(deliveryLocation.DeliveryTime.Hour);
                }
                ViewData["deliveryTime"] = deliveryTime;
                var currentUserId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
                var totalRewardPoints = 0.0m;
                var pts = db.Rewards.Where(i => i.UserId == currentUserId).Sum(i => (decimal?)i.Amount);
                if (pts.HasValue)
                {
                    totalRewardPoints = pts.Value;
                }
                ViewData["totalRewardPoints"] = Convert.ToInt32(totalRewardPoints);
                var order = new Order();
                var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
                var payer = (MyLunchBoxMembershipUser)(new MyLunchBoxMembershipProvider().GetUser(User.Identity.Name, true));
                    
                order.UserId = userId.Value;
                order.OrderReceivedAt = DateTime.Now;
                order.PayerEmail = User.Identity.Name;
                order.Fee = 0.0m;
                order.Gross = cart.ShoppingCartItems.Sum(i => i.LineItemCost);
                //todo: change after add reword points
                if (cart.CanUserRewardPoint )
                {
                    order.rewardPoints = Convert.ToInt32(order.Gross * 100);
                }
                else {
                    order.rewardPoints = 0; 
                }
                order.Savings = order.rewardPoints / 100.0m;
                order.OrderDescription = string.Join(",", cart.ShoppingCartItems.Select(i => i.Description).ToList());
                order.Tax = 0.0m;
                order.PaymentType = PaymentType.NotRequired;
                    
                order.billingFirstName = payer.UserDetails.FirstName;
                order.billingLastName = payer.UserDetails.LastName;
                order.NeedDeliveryInfo = cart.NeedDeliveryInfo;
                if (cart.NeedDeliveryInfo)
                {
                    order.ReceiverFirstName = payer.UserDetails.FirstName;
                    order.ReceiverLastName = payer.UserDetails.LastName;
                    order.ReceiverPhoneNumber = payer.UserDetails.PhoneNumber;
                    order.DeliveryTime = deliveryTime;
                }
                var billingLocation = payer.UserDetails.Location;
                order.billingFirstName = billingLocation != null ? billingLocation.FirstName : "";
                order.billingLastName = billingLocation != null ? billingLocation.LastName : "";
                order.billingAddress1 = billingLocation != null ? billingLocation.Address1 : "";
                order.billingAddress2 = billingLocation != null ? billingLocation.Address2 : "";
                order.billingCity = billingLocation != null ? billingLocation.City : "";
                order.billingState = billingLocation != null ? billingLocation.StateOrProvince : "";
                order.billingZipCode = billingLocation != null ? billingLocation.ZipCode : "";
                return View(order);
                
            }
            catch
            {
                return HttpNotFound();
            }
            
        }

        public ActionResult PaymentComplete(int orderId)
        {
            var order = db.Orders.Single(i => i.OrderId == orderId);
            if (order == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(order);
            }
        }

        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            // fill out basic info
            var cart = ShoppingCartHelper.GetCart(HttpContext);
            ViewData["cart"] = cart;
            var deliveryLocation = LocationHelper.GetDeliveryLocation(HttpContext);
            if (deliveryLocation == null)
            {
                deliveryLocation = db.University_Delivery.First(i => i.UniversityId == 1);
                LocationHelper.SetDeliveryLocation(HttpContext, deliveryLocation);
            }
            ViewData["deliveryLocation"] = deliveryLocation;
            DateTime deliveryTime;
            if (deliveryLocation.DeliveryTime.Hour < DateTime.Now.Hour)
            {
                deliveryTime = DateTime.Now.Date.AddDays(1).AddHours(deliveryLocation.DeliveryTime.Hour);
            }
            else
            {
                deliveryTime = DateTime.Now.Date.AddHours(deliveryLocation.DeliveryTime.Hour);
            }
            ViewData["deliveryTime"] = deliveryTime;
            var totalRewardPoints = db.Rewards.Sum(i => i.Amount);
            ViewData["totalRewardPoints"] = totalRewardPoints;

            //fillout order
            var order = new Order();
            var userId = MembershipHelper.GetUserIdByEmail(HttpContext.User.Identity.Name);
            order.Gross = cart.Gross;
            if (cart.CanUserRewardPoint)
            {
                order.rewardPoints = Convert.ToInt32(values["rewardPoints"]);
            }
            else
            {
                order.rewardPoints = 0;
            }
            order.Savings = order.rewardPoints / 100.0m;
            order.Tax = 0.0m;
            order.Fee = 0.0m;
            order.billingFirstName = values["billingFirstName"];
            order.billingLastName = values["billingLastName"];
            order.billingAddress1 = values["billingAddress1"];
            order.billingAddress2 = values["billingAddress2"];
            order.billingCity = values["billingCity"];
            order.billingState = values["billingState"];
            order.billingZipCode = values["billingZipCode"];
            order.PaymentType = (PaymentType)Enum.Parse(typeof(PaymentType), values["paymentType"]);
            order.cardNumber = values["cardNumber"];
            order.cardExpYear = int.Parse(values["cardExpYear"]);
            order.cardExpMonth = int.Parse(values["cardExpMonth"]);
            order.CSV = values["CSV"];
            order.NeedDeliveryInfo = cart.NeedDeliveryInfo;
            if (cart.NeedDeliveryInfo)
            {
                order.ReceiverFirstName = values["ReceiverFirstName"];
                order.ReceiverLastName = values["ReceiverLastName"];
                order.ReceiverPhoneNumber = values["ReceiverPhoneNumber"];
                order.DeliveryTime = deliveryTime;
            }
            order.PayerEmail = HttpContext.User.Identity.Name;
            order.UserId = userId.Value;
            order.PayerEmail = User.Identity.Name;
            order.OrderDescription = string.Join(",", cart.ShoppingCartItems.Select(i => i.Description).ToList());
            order.DeliveryLocationId = deliveryLocation.LocationId;
            PaymentStatusLevel paymentStatus = PaymentStatusLevel.WaitingForPayment;
            DeliveryInfo deliveryInfo = new DeliveryInfo() { ReceiverFirstName = order.ReceiverFirstName, ReceiverLastName = order.ReceiverLastName };

            if (!TryValidateModel(order))
            {
                return View("AddressAndPayment", "_CheckoutMaster", order);
            }
            else
            {

                if (order.PaymentType == PaymentType.CreditCard)
                {

                    DoDirectPaymentRequestType request = new DoDirectPaymentRequestType();
                    DoDirectPaymentRequestDetailsType requestDetails = new DoDirectPaymentRequestDetailsType();
                    request.DoDirectPaymentRequestDetails = requestDetails;
                    requestDetails.PaymentAction = PaymentActionCodeType.SALE;

                    // Populate card requestDetails
                    CreditCardDetailsType creditCard = new CreditCardDetailsType();
                    requestDetails.CreditCard = creditCard;
                    PayerInfoType payer = new PayerInfoType();
                    PersonNameType name = new PersonNameType();
                    name.FirstName = order.billingFirstName;
                    name.LastName = order.billingLastName;
                    payer.PayerName = name;
                    creditCard.CardOwner = payer;

                    creditCard.CreditCardNumber = order.cardNumber;
                    creditCard.CreditCardType = (CreditCardTypeType)
                        Enum.Parse(typeof(CreditCardTypeType), values["creditCardType"]);
                    creditCard.CVV2 = order.CSV;
                    creditCard.ExpMonth = order.cardExpMonth;
                    creditCard.ExpYear = order.cardExpYear;
                    requestDetails.PaymentDetails = new PaymentDetailsType();
                    AddressType billingAddr = new AddressType();
                    billingAddr.Name = order.billingFirstName + " " + order.billingLastName;
                    billingAddr.Street1 = order.billingAddress1;
                    billingAddr.Street2 = order.billingAddress2;
                    billingAddr.CityName = order.billingCity;
                    billingAddr.StateOrProvince = order.billingState;
                    billingAddr.Country = CountryCodeType.US;
                    billingAddr.PostalCode = order.billingZipCode;
                    payer.Address = billingAddr;
                    // Populate payment requestDetails
                    CurrencyCodeType currency = (CurrencyCodeType)
                        Enum.Parse(typeof(CurrencyCodeType), "USD");
                    BasicAmountType paymentAmount = new BasicAmountType(currency, order.FinalAmount.ToString());
                    requestDetails.PaymentDetails.OrderTotal = paymentAmount;

                    // Invoke the API
                    DoDirectPaymentReq wrapper = new DoDirectPaymentReq();
                    wrapper.DoDirectPaymentRequest = request;

                    PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService();

                    DoDirectPaymentResponseType response = service.DoDirectPayment(wrapper);
                    
                    BillingInfo billingInfo = new BillingInfo()
                    {
                        BillingFirstName = order.billingFirstName,
                        BillingLastName = order.billingLastName,
                        BillingAddress1 = order.billingAddress1,
                        BillingAddress2 = order.billingAddress2,
                        City = order.billingCity,
                        State = order.billingState,
                        ZipCode = order.billingZipCode,
                        CountryCode = CountryCodeType.US
                    };

                    if (response.Ack.Equals(AckCodeType.FAILURE) ||
                        (response.Errors != null && response.Errors.Count > 0))
                    {
                        paymentStatus = PaymentStatusLevel.WaitingForPayment;
                    }
                    else
                    {
                        paymentStatus = PaymentStatusLevel.Paid;
                    }

                    OnPaymentComplete(order.PaymentType, paymentStatus, billingInfo, deliveryInfo, order);

                    if (order.OrderId == 0)
                    {
                        ViewBag.Errors = string.Join(",", response.Errors.Select(i => i.LongMessage).ToList());
                        ViewData["Errors"] = string.Join(",", response.Errors.Select(i => i.LongMessage).ToList());
                        return View("AddressAndPayment", "_CheckoutMaster", order);
                    }
                    return RedirectToAction("PaymentComplete", new { orderId = order.OrderId });
                }
                else if(order.PaymentType == PaymentType.Cash) {
                    paymentStatus = PaymentStatusLevel.WaitingForPayment;
                    OnPaymentComplete(order.PaymentType, paymentStatus, null, deliveryInfo, order);
                    return RedirectToAction("PaymentComplete", new { orderId = order.OrderId });
                }
                else
                {
                    OnPaymentComplete(order.PaymentType, PaymentStatusLevel.Paid, null, deliveryInfo, order);
                    
                    return RedirectToAction("PaymentComplete", new { orderId = order.OrderId });
                }
            }
        }

        public void OnPaymentComplete( PaymentType paymentType, PaymentStatusLevel paymentStatus, BillingInfo billingInfo, DeliveryInfo deliveryInfo, Order order)
        {  
            
            if ( paymentType == PaymentType.CreditCard && paymentStatus == PaymentStatusLevel.WaitingForPayment) 
            {
                // payment failed, we won't record it in db
                order.PaymentStatus = paymentStatus.ToString();
                
            }
            else
            {
                var cart = ShoppingCartHelper.GetCart(HttpContext);
                order.PaymentStatus = paymentStatus.ToString();
                order.TxnId = order.TxnId;
                order.OrderStatusId = (int)OrderStatusLevel.Processing;
                order.PaymentNote = paymentType.ToString() ;
                order.OrderReceivedAt = DateTime.Now;
                db.Orders.AddObject(order);
                db.SaveChanges();
                foreach (var cartItem in cart.ShoppingCartItems)
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        OrderId = order.OrderId,
                        ItemId = cartItem.ItemId,
                        ItemTypeId = cartItem.ItemTypeId,
                        LineItemCost = cartItem.LineItemCost,
                        Quantity = cartItem.Quantity
                    });
                }
                db.SaveChanges();
                
                
                // save payer details 
                var payer = (MyLunchBoxMembershipUser)(new MyLunchBoxMembershipProvider().GetUser(User.Identity.Name, true));
                var userDetails = db.UserDetails.Single(i => i.UserId == order.UserId);
                userDetails.FirstName = order.ReceiverFirstName;
                userDetails.LastName = order.ReceiverLastName;
                userDetails.UniversityId = LocationHelper.GetSelectedUniversityId(HttpContext);
                userDetails.UniversityDeliveryId = LocationHelper.GetDeliveryLocationId(HttpContext);
                if (billingInfo != null)
                {
                    if (userDetails.Location == null)
                    {
                        var location = new Location()
                        {
                            LocationName = billingInfo.BillingAddress1,
                            FirstName = billingInfo.BillingFirstName,
                            LastName = billingInfo.BillingLastName,
                            Address1 = billingInfo.BillingAddress1,
                            Address2 = billingInfo.BillingAddress2,
                            City = billingInfo.City,
                            StateOrProvince = billingInfo.State,
                            ZipCode = billingInfo.ZipCode,
                            CountryCode = billingInfo.CountryCode.ToString(),
                            Country = "United States"
                        };
                        db.Locations.AddObject(location);
                        db.SaveChanges();
                        userDetails.LocationId = location.LocationId;
                    }
                    else
                    {
                        userDetails.Location.FirstName = billingInfo.BillingFirstName;
                        userDetails.Location.LastName = billingInfo.BillingLastName;
                        userDetails.Location.Address1 = billingInfo.BillingAddress1;
                        userDetails.Location.Address2 = billingInfo.BillingAddress2;
                        userDetails.Location.City = billingInfo.City;
                        userDetails.Location.StateOrProvince = billingInfo.State;
                        userDetails.Location.ZipCode = billingInfo.ZipCode;
                    }
                    
                }
                db.UserDetails.ApplyCurrentValues(userDetails);
                db.SaveChanges();
                //process reward
                if (order.rewardPoints > 0)
                {
                    var reward = new Reward()
                    {
                        Amount = -order.rewardPoints,
                        RewardTypeId = (int)RewardType.Order,
                        OrderId = order.OrderId,
                        RewardCreatedAt = DateTime.Now,
                        UserId = MembershipHelper.GetUserIdByEmail(payer.UserName),
                        RewardDescription = "Consume reward points"
                    };
                    db.Rewards.AddObject(reward);
                }
                if(order.OrderItems.Count(i=>i.ItemTypeId == (int)ItemType.MembershipCard) > 0) {
                    var rewardCards = order.OrderItems.Where(i=>i.ItemTypeId == (int)ItemType.MembershipCard);
                    var totalPoints = 0.0m;
                    foreach( var rewardCard in rewardCards) {
                        var card = db.RewardCards.Single(i=>i.RewardCardId == rewardCard.ItemId);
                        totalPoints += card.RewardPoints * rewardCard.Quantity;
                    }
                    var reward = new Reward()
                    {
                        Amount = totalPoints,
                        RewardTypeId = (int)RewardType.RewardCard,
                        OrderId = order.OrderId,
                        Txn = order.TxnId,
                        RewardCreatedAt = DateTime.Now,
                        UserId = MembershipHelper.GetUserIdByEmail(payer.UserName),
                        RewardDescription = "Reward Card Redeemed"
                    };
                    db.Rewards.AddObject(reward);
                }
                db.SaveChanges();

                var oss = new OrderStatusService();
                oss.SendOrderConfirmationMail(order.OrderId);
                // empty shopping cart
                ShoppingCartHelper.EmptyCart(HttpContext);
            }
        }

    }
}

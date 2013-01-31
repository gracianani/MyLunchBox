using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using MyLunchBox.Utilities;
using System.Configuration;

namespace MyLunchBox.Models
{
    public class OrderStatusService
    {
        SmtpClient _mailClient;
        
        public OrderStatusService()
        {
            _mailClient = new MyLunchBoxSMTPClient();
        }
        public bool SendOrderConfirmationMail(int orderId)
        {
            MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
            var myLunchBoxMailer = new MyLunchBoxMailer();
            var client = new MyLunchBoxSMTPClient();
            var order = db.Orders.Single(i => i.OrderId == orderId);
            if (order != null)
            {
                decimal subtotal, discount, paid, cash;
                subtotal = order.Gross;
                discount = order.Savings;
                if (order.PaymentStatus == PaymentStatusLevel.WaitingForPayment.ToString())
                {
                    paid = 0;
                    cash = order.FinalAmount;
                }
                else
                {
                    paid = order.FinalAmount;
                    cash = 0;
                }
                var orderSummary = string.Format(@"<strong>Sub Total</strong>={0}<br />
                                                <strong>Discount</strong>={1}<br />
                                                <strong>Paid</strong>={2}<br /> 
                                                <strong>Cash</strong>=<span style='font-size:1.5em;color:#c84c0b'>{3}</span>",
                                                order.Gross, order.Savings, paid, cash);
                var orderItems = "<tr>" + string.Join("<tr/><tr>", order.OrderItems.Select(i =>
                                                string.Format("<td>{0}</td><td>{1}</td><td>{2}</td>",
                                                i.Item.ItemDescription, i.Quantity, i.LineItemCost))) + "</tr>";
                var deliveryLocation = order.DeliveryLocation.BusinessName;
                var deliveryTime = "";
                if (order.DeliveryTime.HasValue)
                {
                    deliveryTime = order.DeliveryTime.Value.ToString("yyyy-MM-dd HH:mm tt");
                }
                try
                {
                    myLunchBoxMailer.SendTemplateMail(
                    new Dictionary<string, string>() {
                        {"FirstName", order.ReceiverFirstName},
                        {"LastName", order.ReceiverLastName},
                        {"ReceiverPhoneNumber", order.ReceiverPhoneNumber},
                        {"DeliveryLocation", deliveryLocation},
                        {"DeliveryTime", deliveryTime},
                        {"OrderId", order.OrderId.ToString()},
                        {"OrderItems", orderItems},
                        {"OrderSummary", orderSummary}
                    }, "orderConfirmation", order.PayerEmail, ConfigurationManager.AppSettings["SupportEmail"], ConfigurationManager.AppSettings["SupportEmail"], "", client);
                }
                catch {
                    return false;
                }
            }
            return false;
        }
    }
}
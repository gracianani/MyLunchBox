using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLunchBox.Utilities;
using System.Net.Mail;
using System.Net;
using MyLunchBox.Models;

namespace MyLunchBoxTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {

        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) {
        //}
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() {
        // }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanSendOrderConfirmationMail()
        {
            var myLunchBoxMailer = new MyLunchBoxMailer();
            var client = new MyLunchBoxSMTPClient();
            var result = myLunchBoxMailer.SendTemplateMail(
                new Dictionary<string,string>() {
                    {"FirstName", "yaqi"},
                    {"LastName", "zhao"},
                    {"OrderItems", "LunchCombox"},
                    {"OrderSummary", "6.99"}
                }, "orderConfirmation", "yaqi.zhao@elementbars.com", "gracian.ani@gmail.com", "gracian.ani@gmail.com", "", client);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CanSendOrderConfirmationMailByOrderId()
        {
            var orderId = 100;
            var myLunchBoxMailer = new MyLunchBoxMailer();
            var client = new MyLunchBoxSMTPClient();
            var order = db.Orders.Single(i=>i.OrderId == orderId);
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
                                            i.Item.ItemDescription, i.Quantity, i.LineItemCost ))) + "</tr>";
            var deliveryLocation = order.DeliveryLocation.BusinessName;
            var deliveryTime = order.DeliveryTime.ToString("yyyy-MM-dd HH:mm tt");
            var result = myLunchBoxMailer.SendTemplateMail(
                new Dictionary<string, string>() {
                    {"FirstName", order.ReceiverFirstName},
                    {"LastName", order.ReceiverLastName},
                    {"ReceiverPhoneNumber", order.ReceiverPhoneNumber},
                    {"DeliveryLocation", deliveryLocation},
                    {"DeliveryTime", deliveryTime},
                    {"OrderId", order.OrderId.ToString()},
                    {"OrderItems", orderItems},
                    {"OrderSummary", orderSummary}
                }, "orderConfirmation", "yaqi.zhao@elementbars.com", "gracian.ani@gmail.com", "gracian.ani@gmail.com", "", client);
            Assert.AreEqual(true, result);
            // order summary
            //<strong>Sub Total</strong> = $20.0 <br />
            //<strong>Discount</strong> = -$7.0 <br />
            //<strong>Paid</strong> = -$13.0 <br />
            //<strong>Cash</strong> = <span style="font-size:1.5em;color:#c84c0b">$0.00</span> 

            // order items
            //<table cellspacing="0" cellpadding="4" border="1">
            //  <tbody>
            //    <tr valign="top">
            //      <td><strong>Dishes</strong></td>
            //      <td><strong>Quantity</strong></td>
            //      <td><strong>Price ($)</strong></td>
            //    </tr>
            //    <tr valign="top">
            //      <td>Bow<br />
            //      - Colored tender chicken spinach roll<br />
            //      - Corn cups (Big) <br />
            //      </td>
            //      <td>1</td>
            //      <td>5.00</td>
            //    </tr>
            //    <tr valign="top">
            //      <td>2-Entrees Box<br />
            //      - Colored tender chicken spinach roll<br />
            //      - Colored tender chicken spinach roll<br />
            //      - Corn cups (Big) <br />
            //      </td>
            //      <td>1</td>
            //      <td>10.00</td>
            //    </tr>
            //    <tr valign="top">
            //      <td>Fanda</td>
            //      <td>1</td>
            //      <td>5.00</td>
            //    </tr>
            //  </tbody>
            //</table>
        }
    }
}

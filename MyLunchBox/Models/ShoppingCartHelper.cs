using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;

namespace MyLunchBox.Models
{
    public partial class ShoppingCartHelper
    {
        public const string CartSessionKey = "CartId";
        public const string AnonymousUserSessionKey = "AnonymousUserId";
        public static void ClearCartCookie(HttpContextBase context)
        {
            var shoppingCartCookie = new HttpCookie(CartSessionKey);
            shoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(shoppingCartCookie);
        }
        public static int GetCartItemCount(HttpContextBase context)
        {
            try
            {
                var cart = GetCart(context);
                return cart.ShoppingCartItems.Sum(i => i.Quantity);
            }
            catch { return 0; }
        }
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
            var cartId = ShoppingCartHelper.GetCartId(context);
            try
            {
                var shoppingCart = db.ShoppingCarts.Single(i => i.ShoppingCartId == cartId);
                return shoppingCart;
            }
            catch (Exception ex)
            {
                var shoppingCart = new ShoppingCart();
                shoppingCart.CreatedAt = DateTime.Now;
                shoppingCart.LastUpdatedAt = DateTime.Now;
                shoppingCart.UserId = ShoppingCartHelper.GetCurrentUserId(context);
                db.ShoppingCarts.AddObject(shoppingCart);
                db.SaveChanges();

                HttpCookie cartCookie = new HttpCookie(CartSessionKey);
                cartId = shoppingCart.ShoppingCartId;
                cartCookie.Value = shoppingCart.ShoppingCartId.ToString();
                cartCookie.Expires = DateTime.Now.AddYears(2);
                context.Response.Cookies.Add(cartCookie);
                return shoppingCart;
            }
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        // We're using HttpContextBase to allow access to cookies.
        public static int GetCartId(HttpContextBase context)
        {
            int cartId;
            if (context.Request.Cookies[CartSessionKey] == null || context.Request.Cookies[CartSessionKey].Value == null)
            {
                MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
                var shoppingCart = new ShoppingCart();
                shoppingCart.CreatedAt = DateTime.Now;
                shoppingCart.LastUpdatedAt = DateTime.Now;
                shoppingCart.UserId = ShoppingCartHelper.GetCurrentUserId(context);
                db.ShoppingCarts.AddObject(shoppingCart);
                db.SaveChanges();

                HttpCookie cartCookie = new HttpCookie(CartSessionKey);
                cartId = shoppingCart.ShoppingCartId;
                cartCookie.Value = shoppingCart.ShoppingCartId.ToString();
                cartCookie.Expires = DateTime.Now.AddYears(2);
                context.Response.Cookies.Add(cartCookie);
                return cartId;
            }
            else
            {
                return int.Parse(context.Request.Cookies[CartSessionKey].Value.ToString());
            }
            
        }

        public static bool EmptyCart(HttpContextBase context)
        {
            try
            {
                var cart = GetCart(context);
                if (cart.ShoppingCartId != 0)
                {
                    using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "[dbo].[ShoppingCart#Empty]";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("shoppingCartId", cart.ShoppingCartId);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    var cartCookie = new HttpCookie(CartSessionKey);
                    cartCookie.Expires = DateTime.Now.AddDays(-1);
                    context.Response.Cookies.Add(cartCookie);
                    return true;
                }
            }
            catch {
            }
            finally {
                var cartCookie = new HttpCookie(CartSessionKey);
                cartCookie.Expires = DateTime.Now.AddDays(-1);
                context.Response.Cookies.Add(cartCookie);
            };
            return true;
        }
        public static int? GetCurrentUserId(HttpContextBase context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return MembershipHelper.GetUserIdByEmail(context.User.Identity.Name);
            }
            else
            {
                // Generate a new random GUID using System.Guid class
                Guid anonymousUserId = Guid.NewGuid();
                // Send tempCartId back to client as a cookie
                context.Session[AnonymousUserSessionKey] = anonymousUserId.ToString();
            }
            return null;
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their email
        public static void MergeCart(HttpContextBase context, string email)
        {
            MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
            var userId = MembershipHelper.GetUserIdByEmail(email);
            if (userId.HasValue)
            {
                var shoppingCart = db.ShoppingCarts.Single( c => c.UserId == userId.Value );
                
                var unmergedCart = GetCart(context);
                if(shoppingCart != null) {
                    foreach(var shoppingCartItem in shoppingCart.ShoppingCartItems) {
                        unmergedCart.ShoppingCartItems.Add(shoppingCartItem);
                    }
                    db.ShoppingCarts.DeleteObject(shoppingCart);
                }
                db.ShoppingCarts.AddObject(unmergedCart);
                db.SaveChanges();
            }
           
        }

  
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MyLunchBox.Models;
using MyLunchBox.Utilities;

namespace MyLunchBox.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    // assign selected university and selected location by user profile
                    var db = new MyLunchBoxDevelopmentEntities();
                    var userId = MembershipHelper.GetUserIdByEmail(model.Email);
                    if(userId.HasValue) {
                        try {
                            var userDetails = db.UserDetails.Single(i=>i.UserId == userId);
                            if(userDetails.UniversityId.HasValue && LocationHelper.GetSelectedUniversity(HttpContext) == null) {
                                LocationHelper.SetSelectedUniversity(HttpContext, userDetails.University);
                            }
                            if (userDetails.UniversityDeliveryId.HasValue && LocationHelper.GetDeliveryLocation(HttpContext) == null)
                            {
                                LocationHelper.SetDeliveryLocation(HttpContext, userDetails.University_Delivery);
                            }
                        }catch {}
                    }
                    // redirect to returnurl if necessary
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            
            ShoppingCartHelper.ClearCartCookie(HttpContext);
            LocationHelper.ClearSelectedUniversity(HttpContext);
            LocationHelper.ClearDeliveryLocation(HttpContext);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register(string returnUrl)
        {
            var currentUniversity = MyLunchBox.Models.LocationHelper.GetSelectedUniversity(HttpContext);
            var deliveryLocation = MyLunchBox.Models.LocationHelper.GetDeliveryLocation(HttpContext);
            var db = new MyLunchBox.Models.MyLunchBoxDevelopmentEntities();
            var universities = db.Universities.Select(i => new { i.UniversityName, i.UniversityId }).AsEnumerable().Select(i => new SelectListItem() { 
                    Text = i.UniversityName, Value = i.UniversityId.ToString(), Selected = (i.UniversityId == (currentUniversity != null ? currentUniversity.UniversityId : -1)) });
            int currentSelectedUniversityId = currentUniversity == null ? Convert.ToInt32(universities.First().Value) : currentUniversity.UniversityId;
            var universityDeliveries = db.University_Delivery.Where(i => i.UniversityId == currentSelectedUniversityId).Select(i => new { i.UniversityDeliveryId, i.Location.LocationName }).AsEnumerable().Select(i => new SelectListItem() { Text = i.LocationName, Value = i.UniversityDeliveryId.ToString(), Selected = (i.UniversityDeliveryId == (deliveryLocation != null ? deliveryLocation.UniversityDeliveryId : -1)) });
            return View(new RegisterModel() { Universities = universities, UniversityDeliveries = universityDeliveries } );
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            var db = new MyLunchBoxDevelopmentEntities();
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                var user = Membership.CreateUser(model.Email, model.Password, model.Email, model.PasswordQuestion, model.PasswordAnswer, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    var userId = MembershipHelper.GetUserIdByEmail(model.Email);
                    if (userId.HasValue)
                    {
                        var details = db.UserDetails.Single(i => i.UserId == userId.Value);
                        details.FirstName = model.FirstName;
                        details.LastName = model.LastName;
                        details.UniversityId = model.UniversityId;
                        details.UniversityDeliveryId = model.UniversityDeliveryId;
                        db.UserDetails.ApplyCurrentValues(details);
                        db.SaveChanges();
                    }
                    Roles.AddUserToRole(model.Email, MyLunchBoxRoleType.Customer.ToString());
                    FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    if (!string.IsNullOrEmpty(Request.QueryString["returnUrl"]))
                    {
                        return Redirect(Request.QueryString["returnUrl"]);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form

            var universities = db.Universities.Select(i => new { i.UniversityName, i.UniversityId }).AsEnumerable().Select(i => new SelectListItem()
            {
                Text = i.UniversityName,
                Value = i.UniversityId.ToString(),
                Selected = (i.UniversityId == model.UniversityId)
            });
            var universityDeliveries = db.University_Delivery.Where(i => i.UniversityId == model.UniversityId).Select(i => new { i.UniversityDeliveryId, i.Location.LocationName }).AsEnumerable().Select(i => new SelectListItem() { Text = i.LocationName, Value = i.UniversityDeliveryId.ToString(), Selected = (i.UniversityDeliveryId == model.UniversityDeliveryId) });
            model.Universities = universities;
            model.UniversityDeliveries = universityDeliveries;
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword(string oldPassword)
        {
            return View(new ChangePasswordModel() { OldPassword = oldPassword });
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult ForgetPasswordSetEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPasswordSetEmail(ForgetPasswordModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                MembershipUser user = Membership.GetUser(model.Email, true /* userIsOnline */);
                if (user == null)
                {
                    ModelState.AddModelError("", "Can not find user with given email.");
                    return View(model);
                }
                FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                model.PasswordQuestion = user.PasswordQuestion;
                model.PasswordAnswer = "";
                return RedirectToAction("ForgetPassword", new {Email = model.Email });
            }
            return View(model);
        }

        public ActionResult ForgetPassword(string email)
        {
            if(string.IsNullOrEmpty(email)) {
                return View("ForgetPassword");
            }
            MembershipUser user = Membership.GetUser(email, true /* userIsOnline */);
            return View(new ForgetPasswordModel() {Email = email, PasswordQuestion = user.PasswordQuestion});
        }

        [HttpPost]
        public ActionResult ForgetPassword(ForgetPasswordModel model)
        {
            try
            {
                var newPassword = Membership.Provider.ResetPassword(model.Email, model.PasswordAnswer);
                return RedirectToAction("ChangePassword", new ChangePasswordModel() { OldPassword = newPassword });
            }
            catch (Exception ex) {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult GetDeliveryLocations(int universityId)
        {
            var db = new MyLunchBoxDevelopmentEntities();
            var deliveries = db.University_Delivery.Where(i => i.UniversityId == universityId).Select(i => new { i.UniversityDeliveryId, i.Location.LocationName, i.DeliveryTime }).AsEnumerable()
                .Select(i => new SelectListItem() { Text = i.LocationName, Value = i.UniversityDeliveryId.ToString() });
            return PartialView(deliveries);
        }
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

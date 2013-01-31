using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public class LocationHelper
    {
        private static MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        public const string DeliveryLocationSessionKey = "DeliveryLocationId";
        public const string SelectedUniversitySessionKey = "SelectedUniversityId";
        public const string DeliveryTimeSessionKey = "DeliveryTime";

        public static University_Delivery GetDeliveryLocation(HttpContextBase context)
        {
            var universityDeliveryId = LocationHelper.GetDeliveryLocationId(context);
            if (universityDeliveryId.HasValue)
            {
                var deliveryLocation = db.University_Delivery.Single(i => i.UniversityDeliveryId == universityDeliveryId);
                return deliveryLocation;
            }
            return null;
        }
        public static University GetSelectedUniversity(HttpContextBase context)
        {
            var universityId = LocationHelper.GetSelectedUniversityId(context);
            if (universityId.HasValue)
            {
                try
                {
                    var university = db.Universities.Single(i => i.UniversityId == universityId);
                    return university;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static int? GetDeliveryLocationId(HttpContextBase context)
        {
            if (context.Request.Cookies[DeliveryLocationSessionKey] == null || context.Request.Cookies[DeliveryLocationSessionKey].Value == null)
            {
                return null;
            }
            int deliveryLocationId;
            if( int.TryParse(context.Request.Cookies[DeliveryLocationSessionKey].Value.ToString(), out deliveryLocationId)) {
                return deliveryLocationId;
            }
            return null;
        }

        public static int? GetSelectedUniversityId(HttpContextBase context)
        {
            if (context.Request.Cookies[SelectedUniversitySessionKey] == null || context.Request.Cookies[SelectedUniversitySessionKey].Value == null)
            {
                return null;
            }
            int universityId;
            if( int.TryParse(context.Request.Cookies[SelectedUniversitySessionKey].Value.ToString(), out universityId) ){
                return universityId;
            }
            return null;
        }

        public static void SetDeliveryLocation(HttpContextBase context, University_Delivery deli_location)
        {
            HttpCookie deliveryLocation = new HttpCookie(DeliveryLocationSessionKey);
            deliveryLocation.Value = deli_location.UniversityDeliveryId.ToString();
            deliveryLocation.Expires = DateTime.Now.AddYears(2);
            context.Response.Cookies.Add(deliveryLocation);
        }

        public static void SetSelectedUniversity(HttpContextBase context, University university)
        {
            HttpCookie selectedUniversity = new HttpCookie(SelectedUniversitySessionKey);
            selectedUniversity.Value = university.UniversityId.ToString();
            selectedUniversity.Expires = DateTime.Now.AddYears(2);
            context.Response.Cookies.Add(selectedUniversity);
        }

        public static void ClearDeliveryLocation(HttpContextBase context)
        {
            var deliveryLocation = new HttpCookie(DeliveryLocationSessionKey);
            deliveryLocation.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(deliveryLocation);
        }

        public static void ClearSelectedUniversity(HttpContextBase context)
        {
            var selectedUniversity = new HttpCookie(SelectedUniversitySessionKey);
            selectedUniversity.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(selectedUniversity);
        }


    }
}
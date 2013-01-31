using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MyLunchBox.Models;

namespace MyLunchBox.Utilities
{
    public partial class MyLunchBoxMembershipUser : MembershipUser
    {
        private MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();
        private int? _addressId;
        private long? _facebookUserId;
        private string _facebookAccessToken;
        public UserDetail UserDetails
        {
            get
            {
                var userId = MembershipHelper.GetUserIdByEmail(UserName);
                if (db.UserDetails.Count(i => i.UserId == userId) > 0)
                {
                    return db.UserDetails.Single(i => i.UserId == userId);
                }
                else
                {
                    var userDetails = new UserDetail { UserId = userId.Value };
                    db.UserDetails.AddObject(userDetails);
                    db.SaveChanges();
                    return userDetails;
                }
            }
        }
        public string FirstName
        {
            get
            {
                return UserDetails.FirstName;
            }
        }
        public string LastName
        {
            get
            {
                return UserDetails.LastName;
            }
        }
        public string PhoneNumber
        {
            get
            {
                return UserDetails.PhoneNumber;
            }
        }
        public University University
        {
            get
            {
                return UserDetails.University;
            }
        }
        public MyLunchBoxMembershipUser( string providerName,
                                         string userName,
                                         object providerUserKey,
                                         string email,
                                         string passwordQuestion,
                                         string passwordAnswer,
                                         bool isApproved,
                                         bool isLockedOut,
                                         DateTime creationDate,
                                         DateTime lastLoginDate,
                                         DateTime lastActivityDate,
                                         DateTime lastPasswordChangedDate,
                                         DateTime lastLockedOutDate,
                                         int? addressId,
                                         long? facebookUserId,
                                         string facebookAccessToken) :
                                         base(providerName,
                                                userName,
                                                providerUserKey,
                                                email,
                                                passwordQuestion,
                                                passwordAnswer,
                                                isApproved,
                                                isLockedOut,
                                                creationDate,
                                                lastLoginDate,
                                                lastActivityDate,
                                                lastPasswordChangedDate,
                                                lastLockedOutDate)
        {
            _addressId = addressId;
            _facebookUserId = facebookUserId;
            _facebookAccessToken = facebookAccessToken;
        }
    }
}
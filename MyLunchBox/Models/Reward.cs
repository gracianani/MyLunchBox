using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MyLunchBox.Utilities;

namespace MyLunchBox.Models
{
    public partial class Reward
    {
        public string UserName
        {
            get
            {
                if ( UserId.HasValue)
                {
                    var provider = new MyLunchBoxMembershipProvider();
                    var email = MembershipHelper.GetUserEmailById(UserId.Value);
                    return email;
                }
                return null;
            }
        }

        public string WeekSpan
        {
            get
            {
                return string.Format(" {0:MMM dd, yyyy} - {1:MMM dd, yyyy} ", RewardCreatedAt.AddDays(-(int)RewardCreatedAt.DayOfWeek), RewardCreatedAt.AddDays(7 - (int)RewardCreatedAt.DayOfWeek));
            }
        }
    }
}
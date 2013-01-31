using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace MyLunchBox.Models
{
    public partial class RewardCard : IItem
    {

        public int ItemId
        {
            get { return RewardCardId; }
        }

        public int ItemTypeId
        {
            get { return (int)ItemType.MembershipCard; }
        }

        public string ItemDescription
        {
            get { return string.Format("{0} points Membership Card", RewardPoints) ; }
        }

        public string ItemTypeDescription
        {
            get { return ItemType.MembershipCard.ToString(); }
        }

    }
}
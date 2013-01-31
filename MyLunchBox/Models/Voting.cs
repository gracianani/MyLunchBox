using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyLunchBox.Models
{
    public partial class Voting
    {
        public DateTime VotingStartOn
        {
            get
            {
                return DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            }
        }

        public DateTime VotingEndOn
        {
            get
            {
                return DateTime.Now.AddDays(7-(int)DateTime.Now.DayOfWeek);
            }
        }

        public bool IsCurrentWeekVoted( int restaurantId )
        {
            if ( _VotedOn.CompareTo(VotingStartOn) > 0 && Dish.RestaurantId == restaurantId)
            {
                return true;
            }
            return false;
        }

        
    }
}
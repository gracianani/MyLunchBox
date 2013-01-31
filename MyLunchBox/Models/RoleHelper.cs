using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public class RoleHelper
    {
        public static int? GetRoleIdByRoleName(string roleName)
        {
            if (string.Compare(roleName, MyLunchBoxRoleType.Admin.ToString(), true) == 0)
            {
                return 1;
            }
            else if (string.Compare(roleName, MyLunchBoxRoleType.Customer.ToString(), true) == 0)
            {
                return 2;
            }
            else if (string.Compare(roleName, MyLunchBoxRoleType.Restaurant.ToString(), true) == 0)
            {
                return 3;
            }
            return null;
        }
    }
}
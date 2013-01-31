using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public enum OrderStatusLevel
    {
        Pending = 1,
        Processing = 2,
        Holded = 3,
        Complete = 4,
        Closed = 5,
        Canceled = 6
    }
}
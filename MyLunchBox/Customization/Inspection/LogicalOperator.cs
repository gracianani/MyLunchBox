using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Customization
{
    public enum LogicalOperator
    {
        Undefined=0, 
        LessThan = 1,
        LessThanOrEqualTo = 2,
        EqualTo = 3,
        GreaterThanOrEqualTo = 4,
        GreaterThan = 5
    }
}
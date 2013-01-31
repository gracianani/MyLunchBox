using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLunchBox.Models
{
    public interface IItem
    {
        int ItemId { get; }
        int ItemTypeId { get; }
        string ItemDescription { get; }
        string ItemTypeDescription { get; }
        decimal UnitPrice { get; }
    }
}

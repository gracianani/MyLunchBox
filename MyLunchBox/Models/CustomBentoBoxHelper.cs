using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public class CustomBentoBoxHelper
    {
        public static Dictionary<BentoBoxType, List<DishType>> BentoBoxTypeDishTypeMappings = new Dictionary<BentoBoxType, List<DishType>>()
        {
           { BentoBoxType.NoodleAndRice, new List<DishType>() { DishType.NoodleSoup,  DishType.Drink } },
           { BentoBoxType.OneEntreePlate, new List<DishType>() { DishType.MainCourse, DishType.SideDish, DishType.Drink } },
           { BentoBoxType.TwoEntreePlate, new List<DishType>() { DishType.MainCourse, DishType.SideDish, DishType.Drink } },
           { BentoBoxType.ThreeEntreePlate, new List<DishType>() { DishType.MainCourse, DishType.SideDish, DishType.Drink } },
           { BentoBoxType.FourEntreePlate, new List<DishType>() { DishType.MainCourse, DishType.SideDish, DishType.Drink } }
        };
    }
}
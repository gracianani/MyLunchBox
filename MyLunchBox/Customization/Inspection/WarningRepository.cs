using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLunchBox.Models;

namespace MyLunchBox.Customization
{
    public class WarningRepository
    {
        private static List<Inspector> OneEntryWarningRules = new List<Inspector>()
        {
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue= 1, QueryPredicate=(int)Models.DishType.MainCourse, ComparisonLogicalOperator= LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 entree in  your lunch box." },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.SideDish, Message = "You still need a side dish!" },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.MainCourse, Message = "You still need an entree!" },
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue = 1, QueryPredicate=(int)Models.DishType.SideDish, ComparisonLogicalOperator = LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 side dish per lunch box!" }
        };

        private static List<Inspector> TwoEntryWarningRules = new List<Inspector>()
        {
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue= 2, QueryPredicate=(int)Models.DishType.MainCourse, ComparisonLogicalOperator= LogicalOperator.LessThanOrEqualTo, Message = "Only allow 2 entrees in  your lunch box." },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.SideDish, Message = "You still need a side dish!" },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.MainCourse, Message = "You still need an entree!" },
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue = 1, QueryPredicate=(int)Models.DishType.SideDish, ComparisonLogicalOperator = LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 side dish per lunch box!" }
        };

        private static List<Inspector> ThreeEntryWarningRules = new List<Inspector>()
        {
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue= 3, QueryPredicate=(int)Models.DishType.MainCourse, ComparisonLogicalOperator= LogicalOperator.LessThanOrEqualTo, Message = "Only allow 3 entree in  your lunch box." },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.SideDish, Message = "You still need a side dish!" },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.MainCourse, Message = "You still need an entree!" },
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue = 1, QueryPredicate=(int)Models.DishType.SideDish, ComparisonLogicalOperator = LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 side dish per lunch box!" }
        };

        private static List<Inspector> FourEntryWarningRules = new List<Inspector>()
        {
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue= 4, QueryPredicate=(int)Models.DishType.MainCourse, ComparisonLogicalOperator= LogicalOperator.LessThanOrEqualTo, Message = "Only allow 4 entree in  your lunch box." },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.SideDish, Message = "You still need a side dish!" },
            new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.MainCourse, Message = "You still need an entree!" },
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue = 1, QueryPredicate=(int)Models.DishType.SideDish, ComparisonLogicalOperator = LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 side dish per lunch box!" }
        };

        private static List<Inspector> ALaCarteWarningRules = new List<Inspector>()
        {
            new Inspector() { InspectionType = InspectorType.CountOfDishType, CompareToValue= 1, QueryPredicate=(int)Models.DishType.NoodleSoup, ComparisonLogicalOperator= LogicalOperator.LessThanOrEqualTo, Message = "Only allow 1 noodle soup / fry rice in your lunch box." },
             new Inspector() { InspectionType = InspectorType.DishTypeIncluded, QueryPredicate=(int)Models.DishType.NoodleSoup, Message = "You still need a noolde soup or fry rice!" }
        };

        public static Dictionary<BentoBoxType, List<Inspector>> WarningRules = new Dictionary<BentoBoxType, List<Inspector>>() {
            { BentoBoxType.OneEntreePlate, OneEntryWarningRules },
            { BentoBoxType.TwoEntreePlate, TwoEntryWarningRules },
            { BentoBoxType.ThreeEntreePlate, ThreeEntryWarningRules },
            { BentoBoxType.FourEntreePlate, FourEntryWarningRules },
            { BentoBoxType.NoodleAndRice, ALaCarteWarningRules }
        };

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLunchBox.Models;

namespace MyLunchBox.Customization
{
    public class Inspector
    {
        public InspectorType InspectionType{ get; set;}
        public int CompareToValue { get; set; }
        public int QueryPredicate { get; set; }
        public LogicalOperator ComparisonLogicalOperator { get; set; }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public bool Evaluate( CustomBentoBox customBentoBox )
        {
            double inspectionResult;
            switch (InspectionType)
            {
                case InspectorType.DishTypeIncluded :
                    return customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).Count(i => i.DishTypeId == QueryPredicate) > 0;
                case InspectorType.DishTypeNotIncluded :
                    return  customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).Count(i => i.DishTypeId == QueryPredicate) == 0;
                case InspectorType.DishIncluded:
                    return customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).Count(i => i.DishId == QueryPredicate) > 0;
                case InspectorType.DishNotIncluded:
                    return customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).Count(i => i.DishId == QueryPredicate) == 0;
                case InspectorType.CountOfDishType:
                    inspectionResult = customBentoBox.CustomBentoBoxItems.Select(i => i.Dish).Count(i => i.DishTypeId == QueryPredicate || QueryPredicate == 0);
                    break;
                default:
                    return false;
            }
            switch (ComparisonLogicalOperator)
            {
                case LogicalOperator.LessThan:
                    return inspectionResult < CompareToValue;
                case LogicalOperator.LessThanOrEqualTo:
                    return inspectionResult <= CompareToValue;
                case LogicalOperator.EqualTo:
                    return inspectionResult == CompareToValue;
                case LogicalOperator.GreaterThan:
                    return inspectionResult > CompareToValue;
                case LogicalOperator.GreaterThanOrEqualTo:
                    return inspectionResult >= CompareToValue;
                default:
                    return false;
            }
        }
    }
}
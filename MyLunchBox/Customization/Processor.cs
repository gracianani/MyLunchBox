using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLunchBox.Models;

namespace MyLunchBox.Customization
{
    public class Processor
    {
        public List<string> Errors;
        public List<LogicalOperator> ErrorOperators;
        private const string _duplicateErrorMessage = "Dish already added";
        public bool CanAddDish
        {
            get
            {
                if(ErrorOperators != null) {
                    return ErrorOperators.Count(i => i == LogicalOperator.LessThan) + ErrorOperators.Count(i => i == LogicalOperator.LessThanOrEqualTo) == 0;
                }
                return true;
            }
        }
        public void ProcessRule(CustomBentoBox customBentoBox)
        {
            Errors = new List<string>();
            ErrorOperators = new List<LogicalOperator>();
            foreach (var warningRule in WarningRepository.WarningRules[customBentoBox.BentoBox.BentoBoxType])
            {
                if (!warningRule.Evaluate(customBentoBox))
                {
                    Errors.Add(warningRule.Message);
                    ErrorOperators.Add(warningRule.ComparisonLogicalOperator);
                }
            }
        }
        public void AddDuplicationMessage()
        {
            if (!Errors.Contains(_duplicateErrorMessage))
            {
                Errors.Add(_duplicateErrorMessage);
            }
        }
    }
}
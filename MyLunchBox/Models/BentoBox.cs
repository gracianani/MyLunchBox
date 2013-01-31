using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public partial class BentoBox
    {
        public BentoBoxType BentoBoxType
        {
            get
            {
                return (BentoBoxType)Enum.ToObject(typeof(BentoBoxType), _BentoBoxTypeId); 
            }
        }

        public int NumOfEntree
        {
            get
            {
                switch (BentoBoxType)
                {
                    case BentoBoxType.OneEntreePlate:
                        return 1;
                    case BentoBoxType.TwoEntreePlate:
                        return 2;
                    case BentoBoxType.ThreeEntreePlate:
                        return 3;
                    case BentoBoxType.FourEntreePlate:
                        return 4;
                    case BentoBoxType.NoodleAndRice:
                        return 1;
                    default:
                        return 1;
                }
            }
        }

        public int NumOfDishesInDishType(DishType dishType)
        {
            if (dishType == DishType.MainCourse)
            {
                return NumOfEntree;
            }
            else if (dishType == DishType.SideDish)
            {
                return 1;
            }
            else if (dishType == DishType.Drink)
            {
                return 1;
            }
            else if (dishType == DishType.NoodleSoup)
            {
                return 1;
            }
            return 0;
        }

        public List<SelectListItem> BentoBoxStatusLevels
        {
            get
            {
                string[] names = Enum.GetNames(typeof(MyLunchBox.Models.BentoBoxStatusLevel));
                var values = (MyLunchBox.Models.BentoBoxStatusLevel[])Enum.GetValues(typeof(MyLunchBox.Models.BentoBoxStatusLevel));
                var selectList = new List<SelectListItem>();
                for (int i = 0; i < names.Length; i++)
                {
                    selectList.Add(new SelectListItem() { Text = names[i], Value = values[i].ToString(), Selected = _BentoBoxStatusId == (int)values[i] });
                }
                return selectList;
            }
        }
    }
}
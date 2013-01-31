using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Data.Objects;

namespace MyLunchBox.Models
{
    public struct Recipe
    {
        public int BentoBoxId;
        public string DishIds;
    }
    public partial class CustomBentoBox : IItem
    {
        MyLunchBoxDevelopmentEntities db = new MyLunchBoxDevelopmentEntities();

        public CustomBentoBox()
        {
        }

        public CustomBentoBox(string parsedRecipe)
        {
            var recipe = DecodeRecipe( parsedRecipe) ;
            BentoBoxId = recipe.BentoBoxId;
            BentoBox = db.BentoBoxes.Single(i => i.BentoBoxId == BentoBoxId);
            
            var dishIds = recipe.DishIds.Split(',');
            foreach( var item in dishIds) {
                int dishId;
                int quantity = 1;
                var parts = item.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    int.TryParse( parts[1], out quantity);
                }
                if (parts.Length > 0)
                {
                    if(int.TryParse( parts[0], out dishId)) {
                        CustomBentoBoxItems.Add(new CustomBentoBoxItem() { DishId = dishId , Quantity = quantity});  
                    }
                }
            }
        }

        public string EncodedRecipe
        {
            get
            {
                return Json.Encode(new Recipe() { BentoBoxId = BentoBoxId, DishIds = string.Join(",", CustomBentoBoxItems.Select(i=>i.DishId + "-" + i.Quantity).ToList()) });
            }
        }
        public static Recipe DecodeRecipe( string parsedRecipe )
        {
            return (Recipe)Json.Decode(parsedRecipe, typeof( Recipe ));
        }

        public void ChangeQuantity(Dish dish, int quantity)
        {
            var customBentoBoxItem = CustomBentoBoxItems.SingleOrDefault(i => i.DishId == dish.DishId);

            if (customBentoBoxItem == null)
            {
                customBentoBoxItem = new CustomBentoBoxItem
                {
                    DishId = dish.DishId,
                    Quantity = quantity
                };

                CustomBentoBoxItems.Add(customBentoBoxItem);
            }
            else
            {
                customBentoBoxItem.Quantity = quantity;
            }
        }

        public void EmptyCustomBentoBox()
        {
            var dishes = new List<int>();
            dishes.AddRange(CustomBentoBoxItems.Select(i => i.DishId));

            foreach (var dish in dishes)
            {
                var customBentoBoxItems = db.CustomBentoBoxItems.Where(i => i.DishId == dish && i.CustomBentoBoxId == CustomBentoBoxId);
                foreach (var tobedelete in customBentoBoxItems.ToList())
                {
                    db.DeleteObject(tobedelete);
                    CustomBentoBoxItems.Remove(tobedelete);
                }
            }
            db.SaveChanges();
        }
        public void ReplaceRecipe(string dishIds)
        {
            var oldRecipe = new List<int>();
            oldRecipe.AddRange(CustomBentoBoxItems.Select(i => i.DishId));
            foreach (var dishIdVal in dishIds.Split(','))
            {
                int dishId;
                int quantity = 1;
                var parts = dishIdVal.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    int.TryParse(parts[1], out quantity);
                }
                if (parts.Length > 0)
                {
                    if (int.TryParse(parts[0], out dishId))
                    {
                        var dish = db.Dishes.Single(i => i.DishId == dishId);
                        ChangeQuantity(dish, quantity);
                        if( db.CustomBentoBoxItems.Count(i => i.DishId == dishId && i.CustomBentoBoxId == CustomBentoBoxId) == 0) {
                            var dbCustomBentoBoxItem = new CustomBentoBoxItem
                                {
                                    DishId = dish.DishId,
                                    Quantity = quantity,
                                    CustomBentoBoxId = CustomBentoBoxId
                                };
                            db.CustomBentoBoxItems.AddObject(dbCustomBentoBoxItem);
                        }
                        else {
                            var dbCustomBentoBoxItem = db.CustomBentoBoxItems.Single(i=>i.DishId == dishId && i.CustomBentoBoxId == CustomBentoBoxId);
                            dbCustomBentoBoxItem.Quantity = quantity;
                            db.CustomBentoBoxItems.ApplyCurrentValues(dbCustomBentoBoxItem);
                        }
                        db.SaveChanges();
                        oldRecipe.Remove(dish.DishId);
                    }
                }
            }
            foreach (var dishId in oldRecipe)
            {
                var dbCustomBentoBoxItem = db.CustomBentoBoxItems.Single(i => i.DishId == dishId && i.CustomBentoBoxId == CustomBentoBoxId);
                db.CustomBentoBoxItems.DeleteObject(dbCustomBentoBoxItem);
                db.SaveChanges();
                var customBentoBoxItem = CustomBentoBoxItems.Single(i => i.DishId == dishId);
                CustomBentoBoxItems.Remove(customBentoBoxItem);
            }

        }
        public void AddToCustomBentoBox(string dishIds)
        {
            foreach (var dishIdVal in dishIds.Split(','))
            {
                int dishId;
                int quantity = 1;
                var parts = dishIdVal.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    int.TryParse(parts[1], out quantity);
                }
                if (parts.Length > 0)
                {
                    if (int.TryParse(parts[0], out dishId))
                    {
                        var dish = db.Dishes.Single(i=>i.DishId == dishId);
                        ChangeQuantity(dish, quantity);
                    }
                }
            }
        }

        public bool AddToCustomBentoBox(Dish dish)
        {
            var customBentoBoxItem =  CustomBentoBoxItems.SingleOrDefault(i=>i.DishId == dish.DishId);

            if (customBentoBoxItem == null)
            {
                customBentoBoxItem = new CustomBentoBoxItem
                {
                    DishId = dish.DishId,
                    Quantity = 1
                };
                CustomBentoBoxItems.Add(customBentoBoxItem);
                return true;
            }
            return false;
        }
        public void RemoveFromCustomBentoBox(int dishId)
        {
            var customBentoBoxItem = CustomBentoBoxItems.SingleOrDefault(i => i.DishId == dishId);

            if (customBentoBoxItem != null)
            {
                CustomBentoBoxItems.Remove(customBentoBoxItem);
                
            }
        }
        public void RemoveFromCustomBentoBox(Dish dish)
        {
            var customBentoBoxItem = CustomBentoBoxItems.SingleOrDefault(i => i.DishId == dish.DishId);

            if (customBentoBoxItem != null)
            {
                CustomBentoBoxItems.Remove(customBentoBoxItem);
            }
        }

        public int ItemId
        {
            get
            {
                return CustomBentoBoxId;
            }
        }

        public int ItemTypeId
        {
            get
            {
                return (int)ItemType.CustomBentoBox;
            }
        }

        public string ItemDescription
        {
            get
            {
                var items = CustomBentoBoxItems;

                return this.BentoBox.BentoBoxName + ": " + string.Join(",",items.Select(i => i.Dish.DishName).ToList());
            }
        }

        public string ItemTypeDescription
        {
            get
            {
                return BentoBox.BentoBoxName;
            }
        }
        
        public decimal UnitPrice
        {
            get
            {
                var price = BentoBox.UnitPrice + CustomBentoBoxItems.Sum(i => i.Dish.DishIncrementalPrice * i.Quantity);
                return price;
            }
        }

    }
}
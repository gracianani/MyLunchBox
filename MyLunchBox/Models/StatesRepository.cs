using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLunchBox.Models
{
    public class StatesRepository
    {
        private static StatesRepository instance;

        
        private StatesRepository() {
            _states = new Dictionary<string, string>() {
                {"AL", "Alabama"},
	            {"AK","Alaska"},
	            {"AZ","Arizona"},
	            {"AR","Arkansas"},
	            {"CA","California"},
	            {"CO","Colorado"},
	            {"CT","Connecticut"},
	            {"DE","Delaware"},
	            {"DC","District of Columbia"},
	            {"FL","Florida"},
	            {"GA","Georgia"},
	            {"HI","Hawaii"},
	            {"ID","Idaho"},
	            {"IL","Illinois"},
	            {"IN","Indiana"},
	            {"IA","Iowa"},
	            {"KS","Kansas"},
	            {"KY","Kentucky"},
	            {"LA","Louisiana"},
	            {"ME","Maine"},
	            {"MD","Maryland"},
	            {"MA","Massachusetts"},
	            {"MI","Michigan"},
	            {"MN","Minnesota"},
	            {"MS","Mississippi"},
	            {"MO","Missouri"},
	            {"MT","Montana"},
	            {"NE","Nebraska"},
	            {"NV","Nevada"},
	            {"NH","New Hampshire"},
	            {"NJ","New Jersey"},
	            {"NM","New Mexico"},
	            {"NY","New York"},
	            {"NC","North Carolina"},
	            {"ND","North Dakota"},
	            {"OH","Ohio"},
	            {"OK","Oklahoma"},
	            {"OR","Oregon"},
	            {"PA","Pennsylvania"},
	            {"RI","Rhode Island"},
	            {"SC","South Carolina"},
	            {"SD","South Dakota"},
	            {"TN","Tennessee"},
	            {"TX","Texas"},
	            {"UT","Utah"},
	            {"VT","Vermont"},
	            {"VA","Virginia"},
	            {"WA","Washington"},
	            {"WV","West Virginia"},
	            {"WI","Wisconsin"},
	            {"WY","Wyoming"}
            };
        }

        public static StatesRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StatesRepository();
                }
                return instance;
            }
        }

        private Dictionary<string, string> _states;

        public List<SelectListItem> StatesList
        {
            get
            {
                return _states.Select(i => new SelectListItem() { Text = i.Key, Value = i.Value }).ToList();
            }
        }

        public List<SelectListItem> StatesListWithSelectedItem(string selectedKey)
        {
            return _states.Select(i => new SelectListItem() { Text = i.Key, Value = i.Value, Selected = string.Compare(i.Key, selectedKey) == 0 }).ToList(); 
        }
    }
}
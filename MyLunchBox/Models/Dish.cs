using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace MyLunchBox.Models
{
    public partial class Dish
    {
        public List<SelectListItem> DishStatusLevels
        {
            get
            {
                string[] names = Enum.GetNames(typeof(MyLunchBox.Models.DishStatusLevel));
                var values = (MyLunchBox.Models.DishStatusLevel[])Enum.GetValues(typeof(MyLunchBox.Models.DishStatusLevel));
                var selectList = new List<SelectListItem>();
                for (int i = 0; i < names.Length; i++)
                {
                    selectList.Add(new SelectListItem() { Text = names[i], Value = values[i].ToString(), Selected = this.DishStatusId == (int)values[i] });
                }
                return selectList;
            }
        }

        public List<SelectListItem> DishVotingLevels
        {
            get
            {
                string[] names = new string[] { "Voting", "Not voting" };
                var values = new bool[] { true, false };
                var selectList = new List<SelectListItem>();
                for (int i = 0; i < names.Length; i++)
                {
                    selectList.Add(new SelectListItem() { Text = names[i], Value = values[i].ToString(), Selected = this.IsOnVoting == values[i] });
                }
                return selectList;
            }
        }

        public static Dictionary<int, int> GetDishOrderStats(DateTime viewFrom, DateTime viewTo)
        {
            var dishOrderStats = new Dictionary<int, int>();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[DishOrderCount_Fetch]";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("from", viewFrom);
                    cmd.Parameters.AddWithValue("to", viewTo);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dishOrderStats.Add(reader.GetInt32(0), reader.GetInt32(1));
                        }
                    }
                }
            }
            return dishOrderStats;
        }
    }
}
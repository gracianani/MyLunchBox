using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MyLunchBox.Models
{
    public class MembershipHelper
    {
        public static int? GetUserIdByEmail(string email) {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myLunchBox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT userId from [dbo].[users] where email like @email";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 100).Value =  email;
                    conn.Open();
                    var userId = cmd.ExecuteScalar();
                    if (userId != null)
                    {
                        int userIdVal;
                        if( int.TryParse(userId.ToString(), out userIdVal) ) {
                            return userIdVal;
                        }
                    }
                    return null;
                }
            }
        }

        public static string GetUserEmailById(int userId)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myLunchBox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT email from [dbo].[users] where userId = @userId";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    conn.Open();
                    var email = cmd.ExecuteScalar();
                    if (email != null)
                    {
                        return email.ToString();
                    }
                    return null;
                }
            }
        }
    }
}
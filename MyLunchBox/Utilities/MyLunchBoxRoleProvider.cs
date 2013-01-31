using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration.Provider;
using System.Data.SqlClient;
using System.Configuration;
using MyLunchBox.Models;

namespace MyLunchBox.Utilities
{
    public class MyLunchBoxRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (string rolename in roleNames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                if (username.Contains(","))
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in roleNames)
                {
                    if (IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO UsersInRoles (userId, roleId) Values(@userId, @roleId)";
                    var userParam = cmd.Parameters.Add("userId", System.Data.SqlDbType.Int);
                    var roleParam = cmd.Parameters.Add("roleId", System.Data.SqlDbType.Int);
                    int? userId, roleId;
                    SqlTransaction tran = null;
                    try {
                        conn.Open();
                        tran = conn.BeginTransaction();
                        cmd.Transaction = tran;
                        foreach (string username in usernames)
                        {
                            foreach (string rolename in roleNames)
                            {
                                userId = MembershipHelper.GetUserIdByEmail(username);
                                roleId = RoleHelper.GetRoleIdByRoleName(rolename);
                                if (userId.HasValue && roleId.HasValue)
                                {
                                    userParam.Value = userId.Value;
                                    roleParam.Value = roleId.Value;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch { }
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private string _applicationName;
        public override string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationName))
                {
                    _applicationName = "MyLunchBoxRoleProvider";
                }
                return _applicationName;
            }
            set
            {
                _applicationName = value ;
            }
        }

        public override void CreateRole(string roleName)
        {
            if (roleName.Contains(","))
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }

            if (RoleExists(roleName))
            {
                throw new ProviderException("Role name already exists.");
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myLunchBox"].ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Roles (rolename) Values (@rolename)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("rolename", roleName);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            };
            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (!RoleExists(roleName))
            {
                throw new ProviderException("Role does not exist.");
            }
            if (throwOnPopulatedRole && GetUsersInRole(roleName).Length > 0)
            {
                throw new ProviderException("Cannot delete a populated role.");
            }
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Roles where Rolename = @roleName";
                    cmd.Parameters.AddWithValue("roleName", roleName);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            string tmpUserNames = "";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Username FROM UsersInRoles ur JOIN Users usr on usr.userId = ur.userId JOIN Roles rol on rol.roleId = ur.roleId WHERE email LIKE @UsernameSearch AND RoleName = @roleName";
                    cmd.Parameters.AddWithValue("@UsernameSearch",  usernameToMatch);
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            tmpUserNames += reader.GetString(0) + ",";
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            if (tmpUserNames.Length > 0)
            {
                // Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }
            return new string[0];
        }

        public override string[] GetAllRoles()
        {
            string tmpRoleNames = "";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Rolename FROM Roles ";
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tmpRoleNames += reader.GetString(0) + ",";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            if (tmpRoleNames.Length > 0)
            {
                // Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }
            return new string[0];
        }

        public override string[] GetRolesForUser(string username)
        {
            string tmpRoleNames = "";

            using( var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString) ){
                using(var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT Rolename FROM UsersInRoles ur JOIN Users usr ON usr.userId = ur.userId JOIN Roles rol on rol.roleId = ur.roleId WHERE email = @userName";
                    cmd.Parameters.AddWithValue("userName", username);
                    try {
                        conn.Open();
                        using(var reader = cmd.ExecuteReader()) {
                            while (reader.Read())
                            {
                                tmpRoleNames += reader.GetString(0) + ",";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            };
            if (tmpRoleNames.Length > 0)
            {
                // Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        public override string[] GetUsersInRole(string roleName)
        {
            string tmpUserNames = "";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Username FROM UsersInRoles ur JOIN Users usr ON usr.userId = ur.userId JOIN Roles rol on rol.roleId = ur.roleId WHERE Rolename = @rolename";
                    cmd.Parameters.AddWithValue("rolename", roleName);
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tmpUserNames += reader.GetString(0) + ",";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            if (tmpUserNames.Length > 0)
            {
                // Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }
            return new string[0];
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool userIsInRole = false;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*)  FROM UsersInRoles ur JOIN Users usr ON usr.userId = ur.userId JOIN Roles rol on rol.roleId = ur.roleId WHERE Email = @userName AND Rolename = @rolename ";
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("roleName", roleName);
                    try
                    {
                        conn.Open();
                        int numRecs = (int)cmd.ExecuteScalar();
                        if (numRecs > 0)
                        {
                            userIsInRole = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            };
            return userIsInRole;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (string rolename in roleNames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in roleNames)
                {
                    if (!IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is not in role.");
                    }
                }
            }

            using(var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString) ){
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE  FROM UsersInRoles ur JOIN Users usr ON usr.userId = ur.userId JOIN Roles rol on rol.roleId = ur.roleId WHERE email = @username AND Rolename = @roleName";
                    var userParam = cmd.Parameters.Add("username", System.Data.SqlDbType.VarChar);
                    var roleParam = cmd.Parameters.Add("rolename", System.Data.SqlDbType.VarChar);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction();
                        cmd.Transaction = tran;
                        foreach (string username in usernames)
                        {
                            foreach (string rolename in roleNames)
                            {
                                userParam.Value = username;
                                roleParam.Value = rolename;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch { }

                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            
        }

        public override bool RoleExists(string roleName)
        {
            bool exists = false;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mylunchbox"].ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Roles WHERE Rolename = @rolename";
                    cmd.Parameters.AddWithValue("rolename", roleName);
                    try
                    {
                        conn.Open();
                        int numRecs = (int)cmd.ExecuteScalar();
                        if (numRecs > 0)
                        {
                            exists = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return exists;
        }
    }
}
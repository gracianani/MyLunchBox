using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Collections.Specialized;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using System.Configuration.Provider;

namespace MyLunchBox.Utilities
{
    public sealed class MyLunchBoxMembershipProvider : SqlMembershipProvider
    {
        //
        // MembershipProvider.CreateUser
        //
        private string _connectionString = "MyLunchBox";
        private int newPasswordLength = 8;
        public override string Name
        {
            get
            {
                return "MyLunchBoxMembershipProvider";
            }
        }
        public override MembershipUser CreateUser(string username,
                 string password,
                 string email,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved,
                 object strKey,
                 out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }



            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (strKey == null)
                {
                    strKey = Guid.NewGuid();
                }
                else
                {
                    if (!(strKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                var saltBytes = GenerateSaltBytes();

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Users] " +
                      "(email, hashedPassword, salt, strkey, " +
                      " passwordQuestion, passwordAnswer, " +
                      " createdOn, lastActivityAt, lastLoginAt, lastLockoutAt, lastPasswordChangedAt, " +
                      " IsApproved,IsLockedOut, failedPasswordAttemptCount, failedPasswordAnswerAttemptCount, firstName, lastName, receiveEmail, isConfirmed)" +
                      " Values(@email, @Password, @Salt, @strkey, @PasswordQuestion, @PasswordAnswer, @CreatedOn, @LastActivityAt, @LastLoginAt, @LastLockOutAt, @LastPasswordChangedAt, @IsApproved, @IsLockedOut, @FailedPasswordAttemptCount, @FailedPasswordAnswerAttemptCount, '', '', 0, 0 ) SELECT SCOPE_IDENTITY()", conn);

                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@Password", EncodePassword(password, saltBytes));
                cmd.Parameters.AddWithValue("@Salt", Convert.ToBase64String(saltBytes));
                cmd.Parameters.AddWithValue("@Strkey", strKey);
                cmd.Parameters.AddWithValue("@PasswordQuestion", passwordQuestion);
                cmd.Parameters.AddWithValue("@PasswordAnswer", passwordAnswer);
                cmd.Parameters.AddWithValue("@CreatedOn", createDate);
                cmd.Parameters.AddWithValue("@LastActivityAt", createDate);
                cmd.Parameters.AddWithValue("@LastLoginAt", createDate);
                cmd.Parameters.AddWithValue("@LastLockOutAt", createDate);
                cmd.Parameters.AddWithValue("@LastPasswordChangedAt", createDate);
                cmd.Parameters.AddWithValue("@IsApproved",  isApproved);
                cmd.Parameters.AddWithValue("@IsLockedOut", false);
                cmd.Parameters.AddWithValue("@FailedPasswordAttemptCount", 0);
                cmd.Parameters.AddWithValue("@failedPasswordAnswerAttemptCount", 0);

                try
                {
                    conn.Open();

                    var recAdded = cmd.ExecuteScalar();
                    int userId;
                    if (int.TryParse(recAdded.ToString(),out userId))
                    {
                        //create userdetails.
                        cmd = new SqlCommand("INSERT INTO [dbo].[UserDetails](userId) VALUES(@userId)", conn);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        
                        cmd.ExecuteNonQuery();
                        
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (SqlException e)
                {
                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }


                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }


            return null;
        }

        public override bool ChangePassword(string email, string oldPwd, string newPwd)
        {
            if (!ValidateUser(email, oldPwd))
                return false;


            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(email, newPwd, true);

            OnValidatingPassword(args);

            var salt = GetSaltBytesByEmail(email);

            if (args.Cancel) {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }

            if(salt == null) {
                throw new MembershipPasswordException(string.Format("Can not find salt by given email {0}.", email));
            }

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Users] " +
                    " SET HashedPassword = @HashedPassword, LastPasswordChangedAt = @LastPasswordChangedAt" +
                    " WHERE email = @Email", conn);

            cmd.Parameters.AddWithValue("@HashedPassword", EncodePassword(newPwd, salt));
            cmd.Parameters.AddWithValue("@LastPasswordChangedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@Email",  email);


            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        //
        // MembershipProvider.GetUser(string, bool)
        //

        public override MembershipUser GetUser(string email, bool userIsOnline)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT userId, email, strkey," +
                 " IsApproved, IsLockedOut, CreatedOn, LastLoginAt," +
                 " PasswordQuestion, PasswordAnswer," +
                 " LastActivityAt, LastPasswordChangedAt, LastLockoutAt, addressId, facebookUserId, fbAccessToken" +
                 " FROM Users WHERE Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email",  email);

            MembershipUser u = null;
            SqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        reader.Read();
                        u = GetUserFromReader(reader);
                        reader.Close();

                        if (userIsOnline)
                        {
                            using (SqlCommand updateCmd = new SqlCommand("UPDATE [dbo].[Users] " +
                                      "SET LastActivityAt = @LastActivityAt " +
                                      "WHERE Email = @Email", conn))
                            {

                                updateCmd.Parameters.AddWithValue("@LastActivityAt", DateTime.Now);
                                updateCmd.Parameters.AddWithValue("@Email", email);

                                updateCmd.ExecuteNonQuery();
                            }

                        }
                    }
                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }

        //
        // GetUserFromReader
        //    A helper function that takes the current row from the OdbcDataReader
        // and hydrates a MembershipUser from the values. Called by the 
        // MembershipUser.GetUser implementation.
        //

        private MyLunchBoxMembershipUser GetUserFromReader(SqlDataReader reader)
        {
            object strKey = reader.GetString(reader.GetOrdinal("strKey"));
            string email = reader.GetString(reader.GetOrdinal("email"));

            bool isApproved = reader.GetBoolean(reader.GetOrdinal("isApproved"));
            bool isLockedOut = reader.GetBoolean(reader.GetOrdinal("isLockedOut"));
            DateTime createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));

            string passwordQuestion = reader.GetString(reader.GetOrdinal("passwordQuestion"));
            string passwordAnswer = reader.GetString(reader.GetOrdinal("passwordAnswer"));

            DateTime lastLoginAt = new DateTime();
            if (reader.GetValue(reader.GetOrdinal("lastLoginAt")) != DBNull.Value)
                lastLoginAt = reader.GetDateTime(reader.GetOrdinal("lastLoginAt"));

            DateTime lastActivityAt = reader.GetDateTime(reader.GetOrdinal("lastActivityAt"));
            DateTime lastPasswordChangedAt = reader.GetDateTime(reader.GetOrdinal("lastPasswordChangedAt"));

            DateTime lastLockOutAt = new DateTime();
            if (reader.GetValue(reader.GetOrdinal("lastLockoutAt")) != DBNull.Value)
                lastLockOutAt = reader.GetDateTime(reader.GetOrdinal("lastLockoutAt"));

            int? addressId = null;
            if (reader.GetValue(reader.GetOrdinal("addressId")) != DBNull.Value)
                addressId = reader.GetInt32(reader.GetOrdinal("addressId"));

            long? facebookUserId  = null;
            if (reader.GetValue(reader.GetOrdinal("facebookUserId")) != DBNull.Value)
                facebookUserId = reader.GetInt64(reader.GetOrdinal("facebookUserId"));
            
            string facebookAccessToken = null;
            if (reader.GetValue(reader.GetOrdinal("fbAccessToken")) != DBNull.Value)
                facebookAccessToken = reader.GetString(reader.GetOrdinal("fbAccessToken"));
            
            MyLunchBoxMembershipUser u = new MyLunchBoxMembershipUser(this.Name,
                                                  email, strKey, email,
                                                  passwordQuestion, 
                                                  passwordAnswer,
                                                  isApproved,
                                                  isLockedOut,
                                                  createdOn,
                                                  lastLoginAt,
                                                  lastActivityAt,
                                                  lastPasswordChangedAt,
                                                  lastLockOutAt,
                                                  addressId,
                                                  facebookUserId,
                                                  facebookAccessToken);

            return u;
        }

        public override void UpdateUser(MembershipUser user)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Users] " +
                    " IsApproved = @IsApproved" +
                    " WHERE Email = @Email ", conn);

            MyLunchBoxMembershipUser u = (MyLunchBoxMembershipUser)user;

            cmd.Parameters.AddWithValue("@Email",  user.Email);
            cmd.Parameters.AddWithValue("@IsApproved",  user.IsApproved);
            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }
        }

        //
        // MembershipProvider.ValidateUser
        //

        public override bool ValidateUser(string email, string password)
        {
            bool isValid = false;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT HashedPassword, IsApproved FROM [dbo].[Users] WHERE Email = @Email AND IsLockedOut = 0", conn);

            cmd.Parameters.AddWithValue("@Email", email);

            SqlDataReader reader = null;
            bool isApproved = false;
            string pwd = "";

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(reader.GetOrdinal("hashedPassword"));
                    isApproved = reader.GetBoolean(reader.GetOrdinal("isApproved"));
                }
                else
                {
                    return false;
                }

                reader.Close();

                if (CheckPassword(password, pwd, email))
                {
                    if (isApproved)
                    {
                        isValid = true;

                        SqlCommand updateCmd = new SqlCommand("UPDATE Users SET LastLoginAt = @LastLoginAt" +
                                                                " WHERE Email = @Email ", conn);

                        updateCmd.Parameters.AddWithValue("@LastLoginAt",  DateTime.Now);
                        updateCmd.Parameters.AddWithValue("@Email",  email);

                        updateCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    conn.Close();

                    UpdateFailureCount(email, "password");
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            return isValid;
        }

        //
        // MembershipProvider.ResetPassword
        //

        public override string ResetPassword(string email, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            string newPassword =
              System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);


            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(email, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT PasswordAnswer, IsLockedOut FROM Users " +
                  " WHERE Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email", email);

            int rowsAffected = 0;
            string passwordAnswer = "";
            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(reader.GetOrdinal("isLockedOut")))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    passwordAnswer = reader.GetString(reader.GetOrdinal("PasswordAnswer"));
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer, email))
                {
                    UpdateFailureCount(email, "passwordAnswer");

                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                reader.Close();

                SqlCommand updateCmd = new SqlCommand("UPDATE Users " +
                    " SET HashedPassword = @HashedPassword, LastPasswordChangedAt = @LastPasswordChangedAt" +
                    " WHERE Email = @Email AND IsLockedOut = 0", conn);

                updateCmd.Parameters.AddWithValue("@HashedPassword", EncodePassword(newPassword, GetSaltBytesByEmail(email)));
                updateCmd.Parameters.AddWithValue("@LastPasswordChangedAt", DateTime.Now);
                updateCmd.Parameters.AddWithValue("@Email",  email);

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }

        //
        // UpdateFailureCount
        //   A helper method that performs the checks and updates associated with
        // password failure tracking.
        //

        private void UpdateFailureCount(string email, string failureType)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT FailedPasswordAttemptCount, " +
                                              "  FailedPasswordAnswerAttemptCount " +
                                              "  FROM [dbo].[Users] " +
                                              "  WHERE Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email",  email);

            SqlDataReader reader = null;
            int failureCount = 0;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (failureType == "password")
                    {
                        failureCount = reader.GetInt32(reader.GetOrdinal("FailedPasswordAttemptCount"));
                    }

                    if (failureType == "passwordAnswer")
                    {
                        failureCount = reader.GetInt32(reader.GetOrdinal("FailedPasswordAnswerAttemptCount"));
                    }
                }

                reader.Close();

                if (failureCount++ >= MaxInvalidPasswordAttempts)
                {
                    // Password attempts have exceeded the failure threshold. Lock out
                    // the user.

                    cmd.CommandText = "UPDATE Users " +
                                        "  SET IsLockedOut = @IsLockedOut, LastLockedOutDate = @LastLockedOutDate " +
                                        "  WHERE Email = @Email";

                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@IsLockedOut",  true);
                    cmd.Parameters.AddWithValue("@LastLockedOutDate",  DateTime.Now);
                    cmd.Parameters.AddWithValue("@Email",  email);

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new ProviderException("Unable to lock out user.");
                }
                else
                {
                    // Password attempts have not exceeded the failure threshold. Update
                    // the failure counts. Leave the window the same.

                    if (failureType == "password")
                        cmd.CommandText = "UPDATE Users " +
                                            "  SET FailedPasswordAttemptCount = @Count" +
                                            "  WHERE Email = @Email";

                    if (failureType == "passwordAnswer")
                        cmd.CommandText = "UPDATE Users " +
                                            "  SET FailedPasswordAnswerAttemptCount = @Count" +
                                            "  WHERE Email = @Email";

                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@Count",  failureCount);
                    cmd.Parameters.AddWithValue("@Email",  email);

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new ProviderException("Unable to update failure count.");
                }
                
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
        }

        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //

        private bool CheckPassword(string passwordText, string dbpassword, string email)
        {
            string pass1 = passwordText;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    byte[] hashWithSaltBytes = Convert.FromBase64String(dbpassword);
                    int hashSizeInBytes = 20; // we are using SHA1
                    // Allocate array to hold original salt bytes retrieved from hash.
                    byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                                hashSizeInBytes];

                    // Copy salt from the end of the hash to the new array.
                    for (int i = 0; i < saltBytes.Length; i++)
                        saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

                    // Compute a new hash string.
                    pass1 = EncodePassword(passwordText, saltBytes);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }



        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        private string EncodePassword(string passwordText, byte[] saltBytes)
        {
            string encodedPassword = passwordText;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(passwordText)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HashAlgorithm hash = new SHA1Managed();
                    byte[] passwordTextBytes = Encoding.UTF8.GetBytes(passwordText);
                    byte[] passwordTextWithSaltBytes = new byte[passwordTextBytes.Length + saltBytes.Length];
                    // Copy plain text bytes into resulting array.
                    for (int i = 0; i < passwordTextBytes.Length; i++)
                        passwordTextWithSaltBytes[i] = passwordTextBytes[i];

                    // Append salt bytes to the resulting array.
                    for (int i = 0; i < saltBytes.Length; i++)
                        passwordTextWithSaltBytes[passwordTextBytes.Length + i] = saltBytes[i];

                    // Compute hash value of our plain text with appended salt.
                    byte[] hashBytes = hash.ComputeHash(passwordTextWithSaltBytes);

                    // Create array which will hold hash and original salt bytes.
                    byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                        saltBytes.Length];

                    // Copy hash bytes into resulting array.
                    for (int i = 0; i < hashBytes.Length; i++)
                        hashWithSaltBytes[i] = hashBytes[i];

                    // Append salt bytes to the result.
                    for (int i = 0; i < saltBytes.Length; i++)
                        hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

                    // Convert result into a base64-encoded string.
                    encodedPassword = Convert.ToBase64String(hashWithSaltBytes);

                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }


        private byte[] GenerateSaltBytes()
        {
            byte[] saltBytes = new byte[8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);
            return saltBytes;
        }

        private byte[] GetSaltBytesByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Salt FROM [dbo].[Users] WHERE email = @email";
                    cmd.Parameters.AddWithValue("@email", email);
                    conn.Open();
                    var saltString = cmd.ExecuteScalar();
                    if (saltString != null)
                    {
                        return System.Text.Encoding.UTF8.GetBytes(saltString.ToString());
                    }
                    return null;
                } 
            }
        }
    }
}
using System;
using otomobil.Models;
using MySql.Data.MySqlClient;

namespace otomobil.Data
{
    public class UserData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // Single Sql Command
        //public bool CreateUserAccount(User user, UserRole userRole)
        //{
        //	bool result = false;

        //	using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //	{
        //		using (MySqlCommand command = new MySqlCommand())
        //		{
        //			command.Connection = connection;
        //			command.Parameters.Clear();

        //			command.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";

        //			command.Parameters.AddWithValue("@id", user.Id);
        //                  command.Parameters.AddWithValue("@username", user.Username);
        //                  command.Parameters.AddWithValue("@password", user.Password);

        //			command.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";

        //			command.Parameters.AddWithValue("@userId", userRole.UserId);
        //			command.Parameters.AddWithValue("@role", userRole.Role);

        //                  try
        //                  {
        //                      connection.Open();

        //                      int execResult = command.ExecuteNonQuery();

        //                      result = execResult > 0 ? true : false;
        //                  }
        //                  catch
        //                  {
        //                      throw;
        //                  }
        //              }

        //	}

        //           return result;
        //}

        // multiple sql command without transaction
        //public bool CreateUserAccount(User user, UserRole userRole)
        //{

        //    bool result = false;

        //    using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //    {
        //        MySqlCommand command1 = new MySqlCommand();
        //        command1.Connection = connection;
        //        command1.Parameters.Clear();

        //        command1.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";
        //        command1.Parameters.AddWithValue("@id", user.Id);
        //        command1.Parameters.AddWithValue("@username", user.Username);
        //        command1.Parameters.AddWithValue("@password", user.Password);

        //        MySqlCommand command2 = new MySqlCommand();
        //        command2.Connection = connection;
        //        command2.Parameters.Clear();


        //        command2.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";
        //        command2.Parameters.AddWithValue("@userId", userRole.UserId);
        //        command2.Parameters.AddWithValue("@role", userRole.Role);

        //        try
        //        {
        //            connection.Open();

        //            var result1 = command1.ExecuteNonQuery();
        //            var result2 = command2.ExecuteNonQuery();

        //            if (result1 > 0 && result2 > 0) result = true;
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }

        //    return result;

        //}

        // multiple sql command (with transaction)
        public bool CreateUserAccount(User user)
        {
            bool result = false;
            int userId = 0; // Untuk menyimpan user_id yang di-generate oleh database

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    MySqlCommand command1 = new MySqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText = "INSERT INTO users (name, email, password, isActivated) VALUES (@name, @email, @password, @isActivated)";
                    command1.Parameters.AddWithValue("@name", user.name);
                    command1.Parameters.AddWithValue("@email", user.email);
                    command1.Parameters.AddWithValue("@password", user.password);
                    command1.Parameters.AddWithValue("@isActivated", user.isActivated);

                    var result1 = command1.ExecuteNonQuery();

                    // Ambil user_id yang baru saja di-generate oleh database
                    MySqlCommand command2 = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                    userId = Convert.ToInt32(command2.ExecuteScalar());

                    transaction.Commit();  // Commit transaction

                    if (result1 > 0 && userId > 0)
                    {
                        user.user_id = userId;  // Set user_id pada objek User dengan nilai baru
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Handle exception
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }


        public User? CheckUserAuth(string email)
        {
            User? user = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * from users WHERE email = @email";

                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@email", email);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                user_id = Convert.ToInt32(reader["user_id"]),
                                email = reader["email"].ToString() ?? string.Empty,
                                password = reader["password"].ToString() ?? string.Empty,
                                isActivated = Convert.ToBoolean(reader["IsActivated"])
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return user;
        }

        public bool ActivateUser(int user_id)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.Parameters.Clear();

                command.CommandText = "UPDATE users SET isActivated = 1 WHERE user_id = @user_id";
                command.Parameters.AddWithValue("@user_id", user_id);

                connection.Open();
                result = command.ExecuteNonQuery() > 0 ? true : false;

                connection.Close();

            }

            return result;
        }

        public bool ResetPassword(string email, string password)
        {
            bool result = false;

            string query = "UPDATE users SET password = @password WHERE email = @email";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }
    }
}


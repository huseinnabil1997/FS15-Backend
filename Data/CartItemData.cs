using otomobil.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;

namespace otomobil.Data
{
    public class CartItemData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public CartItemData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public bool Insert(CartItem cartItem)
        {
            bool result = false;

            string query = $"INSERT INTO cart_items (user_id, course_id, category_id, course_name, SCHEDULE, price, quantity) " + $"VALUES(@user_id, @course_id, @category_id, @course_name, @SCHEDULE, @price, @quantity)";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@user_id", cartItem.user_id);
                    command.Parameters.AddWithValue("@course_id", cartItem.course_id);
                    command.Parameters.AddWithValue("@category_id", cartItem.category_id);
                    command.Parameters.AddWithValue("@course_name", cartItem.course_name);
                    command.Parameters.AddWithValue("@SCHEDULE", cartItem.schedule);
                    command.Parameters.AddWithValue("@price", cartItem.price);
                    command.Parameters.AddWithValue("@quantity", cartItem.quantity);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;

        }

    }
}


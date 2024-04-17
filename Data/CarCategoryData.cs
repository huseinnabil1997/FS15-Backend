using otomobil.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;

namespace otomobil.Data
{
    public class CarCategoryData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public CarCategoryData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //SelectAll
        public List<CarCategory> GetAll()
        {
            List<CarCategory> carCategories = new List<CarCategory>();
            string query = "SELECT * FROM car_categories;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                carCategories.Add(new CarCategory
                                {
                                    category_id = Convert.ToInt32(reader["category_id"]),
                                    category_name = reader["category_name"].ToString(),
                                    image_url = reader["image_url"].ToString(),
                                });
                            }
                        }
                    }

                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                    

                }

            }

            return carCategories;

        }
    }
}


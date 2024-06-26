﻿using otomobil.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;

namespace otomobil.Data
{
    public class CourseData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public CourseData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //SelectAll
        public List<Course> GetAll()
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT c.course_id, c.course_name, cc.category_name, c.image_url, c.price\r\nFROM courses AS c\r\nJOIN car_categories AS cc ON c.category_id = cc.category_id;";
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
                                courses.Add(new Course
                                {
                                    course_id = Convert.ToInt32(reader["course_id"]),
                                    course_name = reader["course_name"].ToString() ?? string.Empty,
                                    category_name = reader["category_name"].ToString(),
                                    image_url = reader["image_url"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
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

            return courses;

        }

        public List<Course> GetAllByCategoryId(int categoryId)
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT c.course_id, c.course_name, cc.category_name, c.image_url, c.price\r\nFROM courses AS c\r\nJOIN car_categories AS cc ON c.category_id = cc.category_id\r\nWHERE c.category_id = @categoryId;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courses.Add(new Course
                                {
                                    course_id = Convert.ToInt32(reader["course_id"]),
                                    course_name = reader["course_name"].ToString() ?? string.Empty,
                                    category_name = reader["category_name"].ToString(),
                                    image_url = reader["image_url"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
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

            return courses;
        }

        public Course? GetCourseById(int course_id)
        {
            Course? course = null;

            string query = $"SELECT c.course_id, c.category_id, cc.category_name, c.course_name, c.image_url, c.price\r\nFROM courses c\r\nJOIN car_categories cc ON c.category_id = cc.category_id\r\nWHERE c.course_id = @course_id;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@course_id", course_id);

                    connection.Open();


                    using (MySqlDataReader reader = command.ExecuteReader())

                    {
                        while (reader.Read())
                        {
                            course = new Course
                            {
                                course_id = Convert.ToInt32(reader["course_id"]),
                                category_id = Convert.ToInt32(reader["category_id"]),
                                course_name = reader["course_name"].ToString() ?? string.Empty,
                                category_name = reader["category_name"].ToString() ?? string.Empty,
                                image_url = reader["image_url"].ToString(),
                                price = Convert.ToInt32(reader["price"]),
                            };
                        }
                    }
                }

                connection.Close();
            }

            return course;
        }

    }
}


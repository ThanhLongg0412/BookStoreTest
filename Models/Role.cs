using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
    }

    public class RoleModel
    {
        private readonly IConfiguration _configuration;

        public RoleModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, active FROM roles";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Role role = new Role
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Active = Convert.ToBoolean(reader["active"])
                        };
                        roles.Add(role);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return roles;
        }

        public Role GetRoleById(int id)
        {
            Role role = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, active FROM roles WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        role = new Role
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Active = Convert.ToBoolean(reader["active"])
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return role;
        }

        public bool AddRole(string name)
        {
            if (IsRoleExists(name))
            {
                Console.WriteLine("Error: Role with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO roles (name, active) VALUES (@name, @active)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@active", true);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateRole(int id, string name, bool active)
        {
            if (IsRoleExists(name))
            {
                Console.WriteLine("Error: Role with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE roles SET name = @name, active = @active WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@active", active);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool DeleteRole(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM roles WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool IsRoleExists(string name)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM roles WHERE name = @name";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
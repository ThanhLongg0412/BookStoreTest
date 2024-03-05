using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Permission
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
    }

    public class PermissionModel
    {
        private readonly IConfiguration _configuration;

        public PermissionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Permission> GetAllRoles()
        {
            List<Permission> permissions = new List<Permission>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, active FROM permissions";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Permission permission = new Permission
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Active = Convert.ToBoolean(reader["active"])
                        };
                        permissions.Add(permission);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return permissions;
        }

        public Permission GetPermissionById(int id)
        {
            Permission permission = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, active FROM permissions WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        permission = new Permission
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

            return permission;
        }

        public bool AddPermission(Permission permission)
        {
            if (IsPermissionExists(permission.Name))
            {
                Console.WriteLine("Error: Permission with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO permissions (name, active) VALUES (@name, @active)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", permission.Name);
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

        public bool UpdatePermission(int id, Permission permission)
        {
            if (IsPermissionExists(permission.Name))
            {
                Console.WriteLine("Error: Permission with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE permissions SET name = @name, active = @active WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", permission.Name);
                command.Parameters.AddWithValue("@active", permission.Active);
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

        public bool DeletePermission(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM permissions WHERE id = @id";
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

        public bool IsPermissionExists(string name)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM permissions WHERE name = @name";
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

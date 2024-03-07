using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Admin
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public bool RoleActive { get; set; }
    }

    public class AdminModel
    {
        private readonly IConfiguration _configuration;

        public AdminModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Admin> GetAllAdmins()
        {
            List<Admin> admins = new List<Admin>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT admins.id, admins.username, admins.password, " +
                    "admins.email, admins.full_name, admins.role_id, roles.name AS role_name, " +
                    "roles.active AS role_active FROM admins INNER JOIN roles ON " +
                    "admins.role_id = roles.id";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Admin admin = new Admin
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            RoleId = (int)reader["role_id"],
                            RoleName = reader["role_name"].ToString(),
                            RoleActive = Convert.ToBoolean(reader["role_active"])
                        };
                        admins.Add(admin);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return admins;
        }

        public Admin GetAdminById(int id)
        {
            Admin admin = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT admins.id, admins.username, admins.password, " +
                    "admins.email, admins.full_name, admins.role_id, roles.name AS role_name, " +
                    "roles.active AS role_active FROM admins INNER JOIN roles ON " +
                    "admins.role_id = roles.id WHERE admins.id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        admin = new Admin
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["name"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            RoleId = (int)reader["role_id"],
                            RoleName = reader["role_name"].ToString(),
                            RoleActive = Convert.ToBoolean(reader["role_active"])
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return admin;
        }

        public bool AddAdmin(Admin admin)
        {
            if (IsAdminExists(admin.Username, admin.Email))
            {
                Console.WriteLine("Error: Admin with the same username or email already exists.");
                return false;
            }

            if (!IsRoleIdExists(admin.RoleId))
            {
                Console.WriteLine("Error: Role with the provided role_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO admins (username, password, email, full_name, " +
                    "role_id) VALUES (@username, @password, @email, @full_name, @role_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", admin.Username);
                command.Parameters.AddWithValue("@password", admin.Password);
                command.Parameters.AddWithValue("@email", admin.Email);
                command.Parameters.AddWithValue("@full_name", admin.FullName);
                command.Parameters.AddWithValue("@role_id", admin.RoleId);

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

        public bool UpdateAdmin(int id, Admin admin)
        {
            if (IsAdminExists(admin.Username, admin.Email))
            {
                Console.WriteLine("Error: Admin with the same username or email already exists.");
                return false;
            }

            if (!IsRoleIdExists(admin.RoleId))
            {
                Console.WriteLine("Error: Role with the provided role_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE admins SET username = @username, password = @password, " +
                    "email = @email, full_name = @full_name, role_id = @role_id WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", admin.Username);
                command.Parameters.AddWithValue("@password", admin.Password);
                command.Parameters.AddWithValue("@email", admin.Email);
                command.Parameters.AddWithValue("@full_name", admin.FullName);
                command.Parameters.AddWithValue("@role_id", admin.RoleId);
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

        public bool DeleteAdmin(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM admins WHERE id = @id";
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

        public bool IsAdminExists(string username, string email)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM admins WHERE username = @username AND " +
                    "email = @email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);

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

        public bool IsRoleIdExists(int role_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM roles WHERE id = @role_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@role_id", role_id);

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

        public List<Admin> SearchAdmins(string keyword)
        {
            List<Admin> admins = new List<Admin>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT admins.id, admins.username, admins.password, " +
                    "admins.email, admins.full_name, admins.role_id, roles.name AS role_name, " +
                    "roles.active AS role_active FROM admins INNER JOIN roles ON " +
                    "admins.role_id = roles.id WHERE full_name LIKE @keyword OR " +
                    "username LIKE @keyword OR phone_number LIKE @keyword";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Admin admin = new Admin
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            RoleId = (int)reader["role_id"],
                            RoleName = reader["role_name"].ToString(),
                            RoleActive = Convert.ToBoolean(reader["role_active"])
                        };
                        admins.Add(admin);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return admins;
        }
    }
}
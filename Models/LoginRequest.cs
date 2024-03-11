using System.Data.SqlClient;

namespace BookStore.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class LoginModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public Admin Login(LoginRequest loginRequest)
        {
            Admin admin = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT * FROM admins WHERE username = @username AND password = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", loginRequest.Username);
                command.Parameters.AddWithValue("@password", loginRequest.Password);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        admin = new Admin
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            RoleId = (int)reader["role_id"],
                            RoleName = reader["role_name"].ToString()
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
    }
}

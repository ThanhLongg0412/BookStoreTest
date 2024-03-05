using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Address { get; set; }
    }

    public class CustomerModel
    {
        private readonly IConfiguration _configuration;

        public CustomerModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, username, password, email, full_name, " +
                    "phone_number, address FROM customers";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            PhoneNumber = reader["phone_number"].ToString(),
                            Address = reader["address"].ToString()
                        };
                        customers.Add(customer);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return customers;
        }

        public Customer GetCustomerById(int id)
        {
            Customer customer = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, username, password, email, full_name, " +
                    "phone_number, address FROM customers WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["name"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            PhoneNumber = reader["phone_number"] is DBNull ? null : reader["phone_number"].ToString(),
                            Address = reader["address"].ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return customer;
        }

        public bool AddCustomer(string username, string password, string email, 
            string full_name, string phone_number, string address)
        {
            if (IsCustomerExists(username, email, phone_number))
            {
                Console.WriteLine("Error: Customer with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO customers (username, password, email, full_name, " +
                    "phone_number, address) VALUES (@username, @password, @email, @full_name, " +
                    "@phone_number, @address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@full_name", full_name);
                command.Parameters.AddWithValue("@phone_number", phone_number);
                command.Parameters.AddWithValue("@address", address);

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

        public bool UpdateCustomer(int id, string username, string password, string email,
            string full_name, string phone_number, string address)
        {
            if (IsCustomerExists(username, email, phone_number))
            {
                Console.WriteLine("Error: Customer with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE customers SET username = @username, " +
                    "password = @password, email = @email, full_name = @full_name, " +
                    "phone_number = @phone_number, address = @address WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@full_name", full_name);
                command.Parameters.AddWithValue("@phone_number", string.IsNullOrEmpty(phone_number) ? DBNull.Value : (object)phone_number);
                command.Parameters.AddWithValue("@address", address);
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

        public bool DeleteCustomer(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM customers WHERE id = @id";
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

        public bool IsCustomerExists(string username, string email, string phone_number)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM customers WHERE username = @username OR " +
                    "email = @email OR phone_number = @phone_number";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@phone_number", phone_number);

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

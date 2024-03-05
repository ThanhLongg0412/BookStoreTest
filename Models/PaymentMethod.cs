using System.Data.SqlClient;

//test up

namespace BookStore.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PaymentModel
    {
        private readonly IConfiguration _configuration;

        public PaymentModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name FROM payment_methods";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        PaymentMethod paymentMethod = new PaymentMethod
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString()
                        };
                        paymentMethods.Add(paymentMethod);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return paymentMethods;
        }

        public PaymentMethod GetPaymentMethodById(int id)
        {
            PaymentMethod paymentMethod = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name FROM payment_methods WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        paymentMethod = new PaymentMethod
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return paymentMethod;
        }

        public bool AddPaymentMethod(string name)
        {
            if (IsPaymentMethodExists(name))
            {
                Console.WriteLine("Error: Payment method with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO payment_methods (name) VALUES (@name)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);

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

        public bool UpdatePaymentMethod(int id, string name)
        {
            if (IsPaymentMethodExists(name))
            {
                Console.WriteLine("Error: Payment method with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE payment_methods SET name = @name WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
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

        public bool DeletePaymentMethod(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM payment_methods WHERE id = @id";
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

        public bool IsPaymentMethodExists(string name)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM payment_methods WHERE name = @name";
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

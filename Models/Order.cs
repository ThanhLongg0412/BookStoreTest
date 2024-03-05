using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public int PaymentMethodId { get; set; }

        public int AdminId { get; set; }

        public int CustomerId { get; set; }
    }

    public class OrderModel
    {
        private readonly IConfiguration _configuration;

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, status, order_date, payment_method_id, admin_id, " +
                    "customer_id FROM orders";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Status = reader["status"].ToString(),
                            OrderDate = Convert.ToDateTime(reader["order_date"]),
                            PaymentMethodId = (int)reader["payment_method_id"],
                            AdminId = (int)reader["admin_id"],
                            CustomerId = (int)reader["customer_id"]
                        };
                        orders.Add(order);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return orders;
        }

        public Order GetOrderById(int id)
        {
            Order order = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, status, order_date, payment_method_id, admin_id, " +
                    "customer_id FROM orders WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        order = new Order
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Status = reader["status"].ToString(),
                            OrderDate = Convert.ToDateTime(reader["order_date"]),
                            PaymentMethodId = (int)reader["payment_method_id"],
                            AdminId = (int)reader["admin_id"],
                            CustomerId = (int)reader["customer_id"]
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return order;
        }

        public bool AddOrder(Order order)
        {
            if (!IsPaymentMethodIdExists(order.PaymentMethodId))
            {
                Console.WriteLine("Error: Payment method with the provided payment_method_id " +
                    "does not exist.");
                return false;
            }

            if (!IsAdminIdExists(order.AdminId))
            {
                Console.WriteLine("Error: Admin with the provided admin_id does not exist.");
                return false;
            }

            if (!IsCustomerIdExists(order.CustomerId))
            {
                Console.WriteLine("Error: Customer with the provided customer_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO orders (status, order_date, payment_method_id, " +
                    "admin_id, customer_id) VALUES (@status, @order_date, @payment_method_id, " +
                    "@admin_id, @customer_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@payment_method_id", order.PaymentMethodId);
                command.Parameters.AddWithValue("@admin_id", order.AdminId);
                command.Parameters.AddWithValue("@customer_id", order.CustomerId);

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

        public bool UpdateOrder(int id, Order order)
        {
            if (!IsPaymentMethodIdExists(order.PaymentMethodId))
            {
                Console.WriteLine("Error: Payment method with the provided payment_method_id " +
                    "does not exist.");
                return false;
            }

            if (!IsAdminIdExists(order.AdminId))
            {
                Console.WriteLine("Error: Admin with the provided admin_id does not exist.");
                return false;
            }

            if (!IsCustomerIdExists(order.CustomerId))
            {
                Console.WriteLine("Error: Customer with the provided customer_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE orders SET status = @status, order_date = @order_date, " +
                    "payment_method_id = @payment_method_id, admin_id = @admin_id, " +
                    "customer_id = @customer_id WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@payment_method_id", order.PaymentMethodId);
                command.Parameters.AddWithValue("@admin_id", order.AdminId);
                command.Parameters.AddWithValue("@customer_id", order.CustomerId);
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

        public bool DeleteOrder(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM orders WHERE id = @id";
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

        public bool IsPaymentMethodIdExists(int payment_method_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM payment_methods WHERE id = @payment_method_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@payment_method_id", payment_method_id);

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

        public bool IsAdminIdExists(int admin_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM admins WHERE id = @admin_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@admin_id", admin_id);

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

        public bool IsCustomerIdExists(int customer_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM customers WHERE id = @customer_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@customer_id", customer_id);

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

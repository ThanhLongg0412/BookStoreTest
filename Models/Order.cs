using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public string? NameCustomer { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public int PaymentMethodId { get; set; }

        public int? AdminId { get; set; }

        public int? CustomerId { get; set; }

        public string PaymentMethodName { get; set; }

        public string AdminName { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public string CustomerAddress { get; set; }
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
                string query = "SELECT orders.id, orders.status, orders.order_date, " +
                    "orders.name_customer, orders.address, orders.phone_number, " +
                    "orders.payment_method_id, orders.admin_id, orders.customer_id, " +
                    "payment_methods.name AS payment_name, admins.full_name AS admin_name, " +
                    "customers.full_name AS customer_name, customers.email AS customer_email, " +
                    "customers.phone_number AS customer_phone, customers.address AS customer_address " +
                    "FROM orders INNER JOIN payment_methods ON " +
                    "orders.payment_method_id = payment_methods.id INNER JOIN admins ON " +
                    "orders.admin_id = admins.id INNER JOIN customers ON " +
                    "orders.customer_id = customers.id";
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
                            NameCustomer = reader["name_customer"] is DBNull ? null : reader["name_customer"].ToString(),
                            Address = reader["address"] is DBNull ? null : reader["address"].ToString(),
                            PhoneNumber = reader["phone_number"] is DBNull ? null : reader["phone_number"].ToString(),
                            PaymentMethodId = (int)reader["payment_method_id"],
                            AdminId = (int)reader["admin_id"] is DBNull ? null : (int?)reader["admin_id"],
                            CustomerId = (int)reader["customer_id"] is DBNull ? null : (int?)reader["customer_id"],
                            PaymentMethodName = reader["payment_name"].ToString(),
                            AdminName = reader["admin_name"].ToString(),
                            CustomerName = reader["customer_name"].ToString(),
                            CustomerEmail = reader["customer_email"].ToString(),
                            CustomerPhone = reader["customer_phone"] is DBNull ? null : reader["customer_phone"].ToString(),
                            CustomerAddress = reader["customer_address"].ToString()
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
                string query = "SELECT orders.id, orders.status, orders.order_date, " +
                    "orders.name_customer, orders.address, orders.phone_number, " +
                    "orders.payment_method_id, orders.admin_id, orders.customer_id, " +
                    "payment_methods.name AS payment_name, admins.full_name AS admin_name, " +
                    "customers.full_name AS customer_name, customers.email AS customer_email, " +
                    "customers.phone AS customer_phone, customers.address AS customer_address " +
                    "FROM orders INNER JOIN payment_methods ON " +
                    "orders.payment_method_id = payment_methods.id INNER JOIN admins ON " +
                    "orders.admin_id = admins.id INNER JOIN customers ON " +
                    "orders.customer_id = customers.id WHERE orders.id = @id";
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
                            NameCustomer = reader["name_customer"] is DBNull ? null : reader["name_customer"].ToString(),
                            Address = reader["address"] is DBNull ? null : reader["address"].ToString(),
                            PhoneNumber = reader["phone_number"] is DBNull ? null : reader["phone_number"].ToString(),
                            PaymentMethodId = (int)reader["payment_method_id"],
                            AdminId = (int)reader["admin_id"] is DBNull ? null : (int?)reader["admin_id"],
                            CustomerId = (int)reader["customer_id"] is DBNull ? null : (int?)reader["customer_id"],
                            PaymentMethodName = reader["payment_name"].ToString(),
                            AdminName = reader["admin_name"].ToString(),
                            CustomerName = reader["customer_name"].ToString(),
                            CustomerEmail = reader["customer_email"].ToString(),
                            CustomerPhone = reader["customer_phone"] is DBNull ? null : reader["customer_phone"].ToString(),
                            CustomerAddress = reader["customer_address"].ToString()
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
                string query = "INSERT INTO orders (status, order_date, name_customer, address, " +
                    "phone_number, payment_method_id, admin_id, customer_id) VALUES (@status, " +
                    "@order_date, @name_customer, @address, @phone_number, @payment_method_id, " +
                    "@admin_id, @customer_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@name_customer", string.IsNullOrEmpty(order.NameCustomer) ? DBNull.Value : (object)order.NameCustomer);
                command.Parameters.AddWithValue("@address", string.IsNullOrEmpty(order.Address) ? DBNull.Value : (object)order.Address);
                command.Parameters.AddWithValue("@phone_number", string.IsNullOrEmpty(order.PhoneNumber) ? DBNull.Value : (object)order.PhoneNumber);
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
                    "name_customer = @name_customer, address = @address, " +
                    "phone_number = @phone_number, payment_method_id = @payment_method_id, " +
                    "admin_id = @admin_id, customer_id = @customer_id WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@order_date", order.OrderDate);
                command.Parameters.AddWithValue("@name_customer", string.IsNullOrEmpty(order.NameCustomer) ? DBNull.Value : (object)order.NameCustomer);
                command.Parameters.AddWithValue("@address", string.IsNullOrEmpty(order.Address) ? DBNull.Value : (object)order.Address);
                command.Parameters.AddWithValue("@phone_number", string.IsNullOrEmpty(order.PhoneNumber) ? DBNull.Value : (object)order.PhoneNumber);
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

        public bool IsAdminIdExists(int? admin_id)
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

        public bool IsCustomerIdExists(int? customer_id)
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

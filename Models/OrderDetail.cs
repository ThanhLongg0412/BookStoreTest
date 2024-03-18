using System.Data.SqlClient;

namespace BookStore.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }

        public int BookId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Amount { get; set; }

        public string BookName { get; set; }

        public decimal BookPrice { get; set; }

        public string? BookImage { get; set; }
    }

    public class OrderDetailModel
    {
        private readonly IConfiguration _configuration;

        public OrderDetailModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<OrderDetail> GetAllOrderDetails(int id)
        {
            OrderDetail orderDetail = null;
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT order_details.order_id, order_details.book_id, order_details.unit_price, " +
                    "order_details.amount, books.name AS book_name, books.price AS book_price, " +
                    "books.image_url AS book_image FROM order_details INNER JOIN books ON " +
                    "order_details.book_id = books.id WHERE order_details.order_id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        orderDetail = new OrderDetail
                        {
                            OrderId = (int)reader["order_id"],
                            BookId = (int)reader["book_id"],
                            UnitPrice = (decimal)reader["unit_price"],
                            Amount = (int)reader["amount"],

                        };
                        orderDetails.Add(orderDetail);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return orderDetails;
        }

        public bool AddOrderDetail(OrderDetail orderDetail)
        {
            if (!IsOrderIdExists(orderDetail.OrderId))
            {
                Console.WriteLine("Error: Order with the provided order_id does not exist.");
                return false;
            }

            if (!IsBookIdExists(orderDetail.BookId))
            {
                Console.WriteLine("Error: Book with the provided book_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO order_details (order_id, book_id, unit_price, " +
                    "amount) VALUES (@order_id, @book_id, @unit_price, @amount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@order_id", orderDetail.OrderId);
                command.Parameters.AddWithValue("@book_id", orderDetail.BookId);
                command.Parameters.AddWithValue("@unit_price", orderDetail.UnitPrice);
                command.Parameters.AddWithValue("@amount", orderDetail.Amount);

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

        public bool IsOrderIdExists(int order_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM orders WHERE id = @order_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@order_d", order_id);

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

        public bool IsBookIdExists(int book_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM books WHERE id = @book_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@book_id", book_id);

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
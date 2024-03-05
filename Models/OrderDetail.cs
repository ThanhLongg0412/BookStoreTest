using System.Data.SqlClient;

namespace BookStore.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }

        public int BookId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Amount { get; set; }
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

        public List<OrderDetail> GetAllOrderDetails()
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT order_id, book_id, unit_price, amount FROM order_details";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderId = (int)reader["order_id"],
                            BookId = (int)reader["book_id"],
                            UnitPrice = (decimal)reader["unit_price"],
                            Amount = (int)reader["amount"]
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
    }
}
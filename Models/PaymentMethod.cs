using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace bookstore.Models;

public class PaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class PaymentModel
{
    private string connectionString; // Chuỗi kết nối đến cơ sở dữ liệu

    public PaymentModel(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // Phương thức để lấy tất cả các phương thức thanh toán từ cơ sở dữ liệu
    public List<PaymentMethod> GetAllPaymentMethods()
    {
        List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

        using (SqlConnection connection = new SqlConnection(connectionString))
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
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return paymentMethods;
    }

    public PaymentMethod GetPaymentMethodById(int id)
    {
        PaymentMethod paymentMethod = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
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
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return paymentMethod;
    }

    // Phương thức để thêm một phương thức thanh toán mới vào cơ sở dữ liệu
    public bool AddPaymentMethod(PaymentMethod paymentMethod)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO payment_methods (name) VALUES (@name)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", paymentMethod.Name);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }

    // Phương thức để cập nhật thông tin của một phương thức thanh toán trong cơ sở dữ liệu
    public bool UpdatePaymentMethod(PaymentMethod paymentMethod)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "UPDATE payment_methods SET name = @name WHERE id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", paymentMethod.Name);
            command.Parameters.AddWithValue("@id", paymentMethod.Id);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }

    // Phương thức để xóa một phương thức thanh toán khỏi cơ sở dữ liệu
    public bool DeletePaymentMethod(int id)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
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
                // Xử lý ngoại lệ nếu có
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}

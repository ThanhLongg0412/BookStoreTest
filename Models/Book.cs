using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Isbn { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime PublishYear { get; set; }

        public string Publisher { get; set; }

        public string Author { get; set; }

        public int CategoryId { get; set; }
    }

    public class BookModel
    {
        private readonly IConfiguration _configuration;

        public BookModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, isbn, name, price, description, image_url, " +
                    "publish_year, publisher, author, category_id FROM books";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Book book = new Book
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Isbn = reader["isbn"].ToString(),
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Description = reader["description"].ToString(),
                            ImageUrl = reader["image_url"] is DBNull ? null : reader["image_url"].ToString(),
                            PublishYear = Convert.ToDateTime(reader["publish_year"]),
                            Publisher = reader["publisher"].ToString(),
                            Author = reader["author"].ToString(),
                            CategoryId = (int)reader["category_id"]
                        };
                        books.Add(book);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return books;
        }

        public Book GetBookById(int id)
        {
            Book book = null;

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, isbn, name, price, description, image_url, " +
                    "publish_year, publisher, author, category_id FROM books WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        book = new Book
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Isbn = reader["isbn"].ToString(),
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Description = reader["description"].ToString(),
                            ImageUrl = reader["image_url"] is DBNull ? null : reader["image_url"].ToString(),
                            PublishYear = Convert.ToDateTime(reader["publish_year"]),
                            Publisher = reader["publisher"].ToString(),
                            Author = reader["author"].ToString(),
                            CategoryId = (int)reader["category_id"]
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return book;
        }

        public bool AddBook(string isbn, string name, decimal price, string description, 
            string? image_url, DateTime publish_year, string publisher, string author, 
            int category_id)
        {
            if (IsBookExists(isbn, name, image_url))
            {
                Console.WriteLine("Error: Book with the same isbn, name or image already exists.");
                return false;
            }

            if (!IsCategoryIdExists(category_id))
            {
                Console.WriteLine("Error: Category with the provided category_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO books (isbn, name, price, description, image_url" +
                    "publish_year, publisher, author, category_id) VALUES (@isbn, @name, @price, " +
                    "@description, @image_url, @publish_year, @publisher, @author, @category_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@isbn", isbn);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@image_url", image_url ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@publish_year", publish_year);
                command.Parameters.AddWithValue("@publisher", publisher);
                command.Parameters.AddWithValue("@author", author);
                command.Parameters.AddWithValue("@category_id", category_id);

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

        public bool UpdateBook(int id, string isbn, string name, decimal price, string description, 
            string? image_url, DateTime publish_year, string publisher, string author, 
            int category_id)
        {
            if (IsBookExists(isbn, name, image_url))
            {
                Console.WriteLine("Error: Book with the same isbn, name or image already exists.");
                return false;
            }

            if (!IsCategoryIdExists(category_id))
            {
                Console.WriteLine("Error: Category with the provided category_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE books SET isbn = @isbn, name = @name, price = @price, " +
                    "description = @description, image_url = @image_url, " +
                    "publish_year = @publish_year, publisher = @publisher, author = @author, " +
                    "category_id = @category_id WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@isbn", isbn);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@image_url", image_url ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@publish_year", publish_year);
                command.Parameters.AddWithValue("@publisher", publisher);
                command.Parameters.AddWithValue("@author", author);
                command.Parameters.AddWithValue("@category_id", category_id);
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

        public bool DeleteBook(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM books WHERE id = @id";
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

        public bool IsBookExists(string isbn, string name, string? image_url)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM books WHERE isbn = @isbn AND name = @name " +
                    "AND image_url = @image_url";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@isbn", isbn);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@image_url", image_url ?? (object)DBNull.Value);

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

        public bool IsCategoryIdExists(int category_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM categories WHERE id = @category_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@category_id", category_id);

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
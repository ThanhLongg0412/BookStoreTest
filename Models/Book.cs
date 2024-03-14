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

        public string PublishYear { get; set; }

        public string Publisher { get; set; }

        public string Author { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
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
                string query = "SELECT books.id, books.isbn, books.name, books.price, " +
                    "books.description, books.image_url, books.publish_year, books.publisher, " +
                    "books.author, books.category_id, categories.name AS category_name " +
                    "FROM books INNER JOIN categories ON books.category_id = categories.id";
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
                            PublishYear = reader["publish_year"].ToString(),
                            Publisher = reader["publisher"].ToString(),
                            Author = reader["author"].ToString(),
                            CategoryId = (int)reader["category_id"],
                            CategoryName = reader["category_name"].ToString()
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
                string query = "SELECT books.id, books.isbn, books.name, books.price, " +
                    "books.description, books.image_url, books.publish_year, books.publisher, " +
                    "books.author, books.category_id, categories.name AS category_name " +
                    "FROM books INNER JOIN categories ON books.category_id = categories.id " +
                    "WHERE books.id = @id";
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
                            PublishYear = reader["publish_year"].ToString(),
                            Publisher = reader["publisher"].ToString(),
                            Author = reader["author"].ToString(),
                            CategoryId = (int)reader["category_id"],
                            CategoryName = reader["category_name"].ToString()
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

        public bool AddBook(Book book)
        {
            if (IsBookExists(book.Isbn, book.Name, book.ImageUrl))
            {
                Console.WriteLine("Error: Book with the same isbn, name or image already exists.");
                return false;
            }

            if (!IsCategoryIdExists(book.CategoryId))
            {
                Console.WriteLine("Error: Category with the provided category_id does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO books (isbn, name, price, description, image_url, " +
                    "publish_year, publisher, author, category_id) VALUES (@isbn, @name, " +
                    "@price, @description, @image_url, @publish_year, @publisher, @author, " +
                    "@category_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@isbn", book.Isbn);
                command.Parameters.AddWithValue("@name", book.Name);
                command.Parameters.AddWithValue("@price", book.Price);
                command.Parameters.AddWithValue("@description", book.Description);
                command.Parameters.AddWithValue("@image_url", string.IsNullOrEmpty(book.ImageUrl) ? DBNull.Value : (object)book.ImageUrl);
                command.Parameters.AddWithValue("@publish_year", book.PublishYear);
                command.Parameters.AddWithValue("@publisher", book.Publisher);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@category_id", book.CategoryId);

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

        public bool UpdateBook(int id, Book book)
        {
            if (IsBookExists(book.Isbn, book.Name, book.ImageUrl, id))
            {
                Console.WriteLine("Error: Book with the same isbn, name or image already exists.");
                return false;
            }

            if (!IsCategoryIdExists(book.CategoryId))
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
                command.Parameters.AddWithValue("@isbn", book.Isbn);
                command.Parameters.AddWithValue("@name", book.Name);
                command.Parameters.AddWithValue("@price", book.Price);
                command.Parameters.AddWithValue("@description", book.Description);
                command.Parameters.AddWithValue("@image_url", string.IsNullOrEmpty(book.ImageUrl) ? DBNull.Value : (object)book.ImageUrl);
                command.Parameters.AddWithValue("@publish_year", book.PublishYear);
                command.Parameters.AddWithValue("@publisher", book.Publisher);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@category_id", book.CategoryId);
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

        public bool IsBookExists(string isbn, string name, string? image_url, int id = -1)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM books WHERE isbn = @isbn AND name = @name " +
                    "AND image_url = @image_url";
                if (id != -1)
                {
                    query += "AND id != @id";
                }
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

        public List<Book> SearchBooks(string keyword)
        {
            List<Book> books = new List<Book>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT books.id, books.isbn, books.name, books.price, " +
                    "books.description, books.image_url, books.publish_year, books.publisher, " +
                    "books.author, books.category_id, categories.name AS category_name FROM " +
                    "books INNER JOIN categories ON books.category_id = categories.id WHERE " +
                    "isbn LIKE @keyword OR name LIKE @keyword OR author LIKE @keyword OR " +
                    "category_name LIKE @keyword";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

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
                            PublishYear = reader["publish_year"].ToString(),
                            Publisher = reader["publisher"].ToString(),
                            Author = reader["author"].ToString(),
                            CategoryId = (int)reader["category_id"],
                            CategoryName = reader["category_name"].ToString()
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
    }
}
using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
    }

    public class CategoryModel
    {
        private readonly IConfiguration _configuration;

        public CategoryModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, parent_id FROM categories";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            ParentId = reader["parent_id"] is DBNull ? null : (int?)reader["parent_id"]
                        };
                        categories.Add(category);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return categories;
        }

        public List<Category> GetCategoryById(int id)
        {
            Category category = null;
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT id, name, parent_id FROM categories WHERE parent_id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        category = new Category
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            ParentId = reader["parent_id"] is DBNull ? null : (int?)reader["parent_id"]
                        };
                        categories.Add(category);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return categories;
        }

        public bool AddCategory(Category category)
        {
            if (IsCategoryNameExists(category.Name))
            {
                Console.WriteLine("Error: Category with the same name already exists.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO categories (name, parent_id) VALUES (@name, @parent_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", category.Name);
                command.Parameters.AddWithValue("@parent_id", category.ParentId ?? (object)DBNull.Value);

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

        public bool UpdateCategory(int id, Category category)
        {
            if (IsCategoryNameExists(category.Name))
            {
                Console.WriteLine("Error: Category with the same name already exists.");
                return false;
            }

            if (category.ParentId.HasValue && !IsParentIdExists(category.ParentId.Value))
            {
                Console.WriteLine("Error: Parent category does not exist.");
                return false;
            }

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE categories SET name = @name, parent_id = @parent_id " +
                    "WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", category.Name);
                command.Parameters.AddWithValue("@parent_id", category.ParentId ?? (object)DBNull.Value);
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

        public bool DeleteCategory(int id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM categories WHERE id = @id";
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

        public bool IsCategoryNameExists(string name)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM categories WHERE name = @name";
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

        public bool IsParentIdExists(int? parent_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT COUNT(*) FROM categories WHERE parent_id = @parent_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@parent_id", parent_id ?? (object)DBNull.Value);

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
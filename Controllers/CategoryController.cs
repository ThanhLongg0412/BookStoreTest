using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        private DataTable ExecuteQuery(string query, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        [HttpGet]
        [Route("GetCategory")]
        public ActionResult GetCategory()
        {
            try
            {
                string query = "SELECT * FROM categories";
                DataTable table = ExecuteQuery(query, null);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCategoryById")]
        public ActionResult GetCategoryById(int id)
        {
            try
            {
                string query = "SELECT * FROM categories WHERE id = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@id", id)
                };
                DataTable table = ExecuteQuery(query, parameters);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("AddCategory")]
        public ActionResult AddCategory([FromForm] string name, [FromForm] int? parent_id)
        {
            try
            {
                // Kiểm tra tính hợp lệ của parent_id
                if (parent_id != null && !IsParentCategoryExists(parent_id.Value))
                {
                    return BadRequest("Parent category does not exist!");
                }

                // Kiểm tra trùng lặp category name
                if (IsDuplicateCategoryName(name))
                {
                    return BadRequest("Category name already exists!");
                }

                // Nếu parent_id hợp lệ, thêm mới Category
                string query = "INSERT INTO categories VALUES (@name, @parent_id)";
                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@parent_id", (object)parent_id ?? DBNull.Value)
                };
                ExecuteNonQuery(query, parameters);

                return new JsonResult("Add Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateCategory")]
        public IActionResult UpdateCategory(int id, [FromForm] string name, [FromForm] int parent_id)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || id <= 0)
                {
                    return BadRequest("Invalid category ID or name!");
                }

                // Kiểm tra trùng lặp tên danh mục
                if (IsDuplicateCategoryName(name))
                {
                    return BadRequest("Category name already exists!");
                }

                string query = "UPDATE categories SET name = @name, parent_id = @parent_id WHERE id = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@parent_id", parent_id),
                    new SqlParameter("@id", id)
                };
                ExecuteNonQuery(query, parameters);

                return new JsonResult("Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                string query = "DELETE FROM categories WHERE id = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@id", id)
                };
                ExecuteNonQuery(query, parameters);
                return new JsonResult("Delete Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        private void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private T ExecuteScalar<T>(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        private bool IsParentCategoryExists(int parent_id)
        {
            string query = "SELECT COUNT(*) FROM categories WHERE id = @parent_id";
            SqlParameter[] parameter = {
                new SqlParameter("@parent_id", parent_id)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameter));
            return count > 0;
        }

        private bool IsDuplicateCategoryName(string name)
        {
            string query = "SELECT COUNT(*) FROM categories WHERE name = @name";
            SqlParameter[] parameters = {
                new SqlParameter("@name", name)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }
    }
}

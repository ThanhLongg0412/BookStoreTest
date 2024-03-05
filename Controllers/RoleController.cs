using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RoleController(IConfiguration configuration)
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
        [Route("GetRole")]
        public ActionResult GetRole()
        {
            try
            {
                string query = "SELECT * FROM roles";
                DataTable table = ExecuteQuery(query, null);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetRoleById")]
        public ActionResult GetRoleById(int id)
        {
            try
            {
                string query = "SELECT * FROM roles WHERE id = @id";
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
        [Route("AddRole")]
        public ActionResult AddRole([FromForm] string name, [FromForm] bool active)
        {
            try
            {
                // Kiểm tra trùng lặp tên vai trò
                if (IsDuplicateRoleName(name))
                {
                    return BadRequest("Role name already exists!");
                }

                string query = "INSERT INTO roles VALUES (@name, @active)";
                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@active", active)
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
        [Route("UpdateRole")]
        public IActionResult UpdateRole(int id, [FromForm] string name, [FromForm] bool active)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || id <= 0)
                {
                    return BadRequest("Invalid role ID or name!");
                }

                // Kiểm tra trùng lặp tên vai trò
                if (IsDuplicateRoleName(name))
                {
                    return BadRequest("Role name already exists!");
                }

                string query = "UPDATE roles SET name = @name, active = @active WHERE id = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@active", active),
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
        [Route("DeleteRole")]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                string query = "DELETE FROM roles WHERE id = @id";
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

        private bool IsDuplicateRoleName(string name)
        {
            string query = "SELECT COUNT(*) FROM roles WHERE name = @name";
            SqlParameter[] parameters = {
                new SqlParameter("@name", name)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }
    }
}

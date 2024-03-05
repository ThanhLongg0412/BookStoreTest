using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PermissionController(IConfiguration configuration)
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
        [Route("GetPermission")]
        public ActionResult GetPermission()
        {
            try
            {
                string query = "SELECT * FROM permissions";
                DataTable table = ExecuteQuery(query, null);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPermissionById")]
        public ActionResult GetPermissionById(int id)
        {
            try
            {
                string query = "SELECT * FROM permissions WHERE id = @id";
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
        [Route("AddPermission")]
        public ActionResult AddPermission([FromForm] string name, [FromForm] bool active)
        {
            try
            {
                // Kiểm tra trùng lặp tên quyền
                if (IsDuplicatePermissionName(name))
                {
                    return BadRequest("Permission name already exists!");
                }

                string query = "INSERT INTO permissions VALUES (@name, @active)";
                SqlParameter[] parameters = {
                    new SqlParameter("@name", name),
                    new SqlParameter("@active", active)
                };
                ExecuteQuery(query, parameters);
                return new JsonResult("Add Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdatePermission")]
        public IActionResult UpdateRole(int id, [FromForm] string name, [FromForm] bool active)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || id <= 0)
                {
                    return BadRequest("Invalid permission ID or name!");
                }

                // Kiểm tra trùng lặp tên quyền
                if (IsDuplicatePermissionName(name))
                {
                    return BadRequest("Permission name already exists!");
                }

                string query = "UPDATE permissions SET name = @name, active = @active WHERE id = @id";

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
        [Route("DeletePermission")]
        public ActionResult DeletePermisson(int id)
        {
            try
            {
                string query = "DELETE FROM permissons WHERE id = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@id", id)
                };
                ExecuteQuery(query, parameters);
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

        private bool IsDuplicatePermissionName(string name)
        {
            string query = "SELECT COUNT(*) FROM permissions WHERE name = @name";
            SqlParameter[] parameters = {
                new SqlParameter("@name", name)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }
    }
}

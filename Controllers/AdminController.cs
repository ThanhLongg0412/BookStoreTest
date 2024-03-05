using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
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
        [Route("GetAdmin")]
        public ActionResult GetAdmin()
        {
            try
            {
                string query = "SELECT * FROM admins";
                DataTable table = ExecuteQuery(query, null);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAdminById")]
        public ActionResult GetAdminById(int id)
        {
            try
            {
                string query = "SELECT * FROM admins WHERE id = @id";
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
        [Route("AddAdmin")]
        public ActionResult AddAdmin([FromForm] string username, [FromForm] string password,
            [FromForm] string email, [FromForm] string fullname, [FromForm] int role_id)
        {
            try
            {
                

                string query = "INSERT INTO admins VALUES " +
                    "(@username, @password, @email, @full_name, @role_id)";
                SqlParameter[] parameters = {
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", password),
                    new SqlParameter("@email", email),
                    new SqlParameter("@full_name", fullname)
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
        [Route("UpdateAdmin")]
        public IActionResult UpdateAdmin(int id, [FromForm] string username,
            [FromForm] string password, [FromForm] string email, [FromForm] string fullname,
            [FromForm] int role_id)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname) || id <= 0)
                {
                    return BadRequest("Invalid ID or information entered!");
                }

                string query = "UPDATE admins SET username = @username, " +
                    "password = @password, email = @email, full_name = @full_name, " +
                    "role_id = @role_id WHERE id = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", password),
                    new SqlParameter("@email", email),
                    new SqlParameter("@full_name", fullname),
                    new SqlParameter("@role_id", role_id),
                    new SqlParameter("@id", id)
                };
                ExecuteNonQuery(query, parameters);

                return new JsonResult("Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCustomer")]
        public ActionResult DeleteCustomer(int id)
        {
            try
            {
                string query = "DELETE FROM customers WHERE id = @id";
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

        
    }
}

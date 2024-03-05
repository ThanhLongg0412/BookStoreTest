using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using BookStore.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
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
        [Route("GetCustomer")]
        public ActionResult GetCustomer()
        {
            try
            {
                string query = "SELECT * FROM customers";
                DataTable table = ExecuteQuery(query, null);
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCustomerById")]
        public ActionResult<Customer> GetCustomerById(int id)
        {
            try
            {
                string query = "SELECT * FROM customers WHERE id = @id";
                SqlParameter[] parameters = {
                    new SqlParameter("@id", id)
                };
                DataTable table = ExecuteQuery(query, parameters);
                /*return new JsonResult(table);*/
                if (table != null && table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    Customer customer = new Customer
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Username = row["username"].ToString(),
                        Password = row["password"].ToString(),
                        Email = row["email"].ToString(),
                        FullName = row["full_name"].ToString(),
                        PhoneNumber = row["phone_number"].ToString(),
                        Address = row["address"].ToString()
                    };
                    return customer;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("AddCustomer")]
        public ActionResult AddCustomer([FromForm] string username, [FromForm] string password, 
            [FromForm] string email, [FromForm] string fullname, [FromForm] string phone_number, 
            [FromForm] string address)
        {
            try
            {
                // Kiểm tra trùng lặp username
                if (IsDuplicateUsername(username))
                {
                    return BadRequest("Username already exists!");
                }

                // Kiểm tra trùng lặp email
                if (IsDuplicateEmail(email))
                {
                    return BadRequest("Email already exists!");
                }

                // Kiểm tra trùng lặp số điện thoại
                if (IsDuplicatePhoneNumber(phone_number))
                {
                    return BadRequest("Phone number already exists!");
                }

                string query = "INSERT INTO customers VALUES " +
                    "(@username, @password, @email, @full_name, @phone_number, @address)";
                SqlParameter[] parameters = {
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", password),
                    new SqlParameter("@email", email),
                    new SqlParameter("@full_name", fullname),
                    new SqlParameter("@phone_number", phone_number),
                    new SqlParameter("@address", address)
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
        [Route("UpdateCustomer")]
        public IActionResult UpdateCustomer(int id, [FromForm] string username,
    [FromForm] string password, [FromForm] string email, [FromForm] string fullname,
    [FromForm] string phone_number, [FromForm] string address)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname) ||
                    string.IsNullOrEmpty(phone_number) || string.IsNullOrEmpty(address) ||
                    id <= 0)
                {
                    return BadRequest("Invalid ID or information entered!");
                }

                // Kiểm tra trùng lặp username
                if (IsDuplicateUsername(username))
                {
                    return BadRequest("Username already exists!");
                }

                // Kiểm tra trùng lặp email
                if (IsDuplicateEmail(email))
                {
                    return BadRequest("Email already exists!");
                }

                // Kiểm tra trùng lặp số điện thoại
                if (IsDuplicatePhoneNumber(phone_number))
                {
                    return BadRequest("Phone number already exists!");
                }

                string query = "UPDATE customers SET username = @username, " +
                    "password = @password, email = @email, full_name = @full_name, " +
                    "phone_number = @phone_number, address = @address WHERE id = @id";

                SqlParameter[] parameters = {
            new SqlParameter("@username", username),
            new SqlParameter("@password", password),
            new SqlParameter("@email", email),
            new SqlParameter("@full_name", fullname),
            new SqlParameter("@phone_number", phone_number),
            new SqlParameter("@address", address),
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

        /*[HttpPut]
        [Route("UpdateCustomer")]
        public IActionResult UpdateCustomer(int id, [FromBody] CustomerUpdate model)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID!");
                }

                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                ActionResult<Customer> customerResult = GetCustomerById(id);
                if (customerResult.Result is BadRequestObjectResult)
                {
                    return BadRequest("Customer not found!");
                }

                Customer customer = customerResult.Value;

                // Kiểm tra trùng lặp username
                if (!string.IsNullOrEmpty(model.Username) && model.Username != customer.Username)
                {
                    if (IsDuplicateUsername(model.Username))
                    {
                        return BadRequest("Username already exists!");
                    }
                    customer.Username = model.Username;
                }

                // Kiểm tra trùng lặp email
                if (!string.IsNullOrEmpty(model.Email) && model.Email != customer.Email)
                {
                    if (IsDuplicateEmail(model.Email))
                    {
                        return BadRequest("Email already exists!");
                    }
                    customer.Email = model.Email;
                }

                // Kiểm tra trùng lặp số điện thoại
                if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber != customer.PhoneNumber)
                {
                    if (IsDuplicatePhoneNumber(model.PhoneNumber))
                    {
                        return BadRequest("Phone number already exists!");
                    }
                    customer.PhoneNumber = model.PhoneNumber;
                }

                // Cập nhật thông tin khách hàng từ model
                if (model.Password != null)
                {
                    customer.Password = model.Password;
                }

                if (model.FullName != null)
                {
                    customer.FullName = model.FullName;
                }

                if (model.Address != null)
                {
                    customer.Address = model.Address;
                }

                string query = "UPDATE customers SET username = @username, " +
                    "password = @password, email = @email, full_name = @full_name, " +
                    "phone_number = @phone_number, address = @address WHERE id = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@username", customer.Username),
                    new SqlParameter("@password", customer.Password),
                    new SqlParameter("@email", customer.Email),
                    new SqlParameter("@full_name", customer.FullName),
                    new SqlParameter("@phone_number", customer.PhoneNumber),
                    new SqlParameter("@address", customer.Address),
                    new SqlParameter("@id", id)
                };

                ExecuteNonQuery(query, parameters);

                return new JsonResult("Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }*/

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

        private bool IsDuplicateUsername(string username)
        {
            string query = "SELECT COUNT(*) FROM customers WHERE username = @username";
            SqlParameter[] parameters = {
                new SqlParameter("@username", username)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }

        private bool IsDuplicateEmail(string email)
        {
            string query = "SELECT COUNT(*) FROM customers WHERE email = @email";
            SqlParameter[] parameters = {
                new SqlParameter("@email", email)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }

        private bool IsDuplicatePhoneNumber(string phone_number)
        {
            string query = "SELECT COUNT(*) FROM customers WHERE phone_number = @phone_number";
            SqlParameter[] parameters = {
                new SqlParameter("@phone_number", phone_number)
            };
            int count = Convert.ToInt32(ExecuteScalar<int>(query, parameters));
            return count > 0;
        }
    }
}

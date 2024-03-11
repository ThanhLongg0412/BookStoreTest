using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerModel _customerModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _customerModel = new CustomerModel(configuration);
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = _customerModel.GetAllCustomers();
            return Ok(customers);
        }

        [HttpGet("id={id}")]
        public IActionResult GetCustomerById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            var customer = _customerModel.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Username) || string.IsNullOrEmpty(customer.Password)
                || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.FullName)
                || string.IsNullOrEmpty(customer.Address))
            {
                return BadRequest("Customer information is incomplete.");
            }

            if (_customerModel.AddCustomer(customer))
            {
                return Ok("Customer added successfully.");
            }
            else
            {
                return BadRequest("Failed to add customer.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            if (string.IsNullOrEmpty(customer.Username) || string.IsNullOrEmpty(customer.Password)
                || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.FullName)
                || string.IsNullOrEmpty(customer.Address))
            {
                return BadRequest("Customer information is incomplete.");
            }

            if (_customerModel.UpdateCustomer(id, customer))
            {
                return Ok("Customer updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update customer.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeleteCustomer(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            if (_customerModel.DeleteCustomer(id))
            {
                return Ok("Customer deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete customer.");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchCustomers(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword cannot be empty");
            }

            var customers = _customerModel.SearchCustomers(keyword);
            return Ok(customers);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] CustomerLoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request.");
            }

            var customer = _customerModel.ValidateCustomerCredentials(loginRequest.Username, loginRequest.Password);

            if (customer != null)
            {
                // Store admin information in session
                _httpContextAccessor.HttpContext.Session.SetString("CustomerUsername", customer.Username);
                _httpContextAccessor.HttpContext.Session.SetInt32("CustomerId", customer.Id);

                return Ok(new { success = true, redirectUrl = "/customer/redirect" });
            }
            else
            {
                return BadRequest(new { success = false, errorMessage = "Invalid username or password" });
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Clear admin information from session
            _httpContextAccessor.HttpContext.Session.Remove("CustomerUsername");
            _httpContextAccessor.HttpContext.Session.Remove("CustomerId");

            return Ok(new { success = true, redirectUrl = "/customer/login" });
        }

        [HttpGet("redirect")]
        public IActionResult RedirectCustomer()
        {
            if (IsCustomerLoggedIn())
            {
                // Admin is logged in, you can redirect to the admin dashboard or perform other actions
                return Ok("Customer is logged in.");
            }
            else
            {
                return Unauthorized("Customer is not logged in.");
            }
        }

        private bool IsCustomerLoggedIn()
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId").HasValue;
        }

        public class CustomerLoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
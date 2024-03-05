using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerModel _customerModel;

        public CustomerController(IConfiguration configuration)
        {
            _customerModel = new CustomerModel(configuration);
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
        public IActionResult AddCustomer([FromBody] string username, [FromBody] string password, 
            [FromBody] string email, [FromBody] string full_name, [FromBody] string phone_number, 
            [FromBody] string address)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Customer username is required.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Customer password is required.");
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Customer email is required.");
            }

            if (string.IsNullOrEmpty(full_name))
            {
                return BadRequest("Customer full name is required.");
            }

            if (string.IsNullOrEmpty(address))
            {
                return BadRequest("Customer address is required.");
            }

            if (_customerModel.AddCustomer(username, password, email, full_name, phone_number, 
                address))
            {
                return Ok("Customer added successfully.");
            }
            else
            {
                return BadRequest("Failed to add customer.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] string username, 
            [FromBody] string password, [FromBody] string email, [FromBody] string full_name, 
            [FromBody] string phone_number, [FromBody] string address)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Customer username is required.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Customer password is required.");
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Customer email is required.");
            }

            if (string.IsNullOrEmpty(full_name))
            {
                return BadRequest("Customer full name is required.");
            }

            if (string.IsNullOrEmpty(address))
            {
                return BadRequest("Customer address is required.");
            }

            if (_customerModel.UpdateCustomer(id, username, password, email, full_name, 
                phone_number, address))
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
    }
}

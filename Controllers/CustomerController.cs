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
    }
}
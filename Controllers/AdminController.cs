using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminModel _adminModel;

        public AdminController(IConfiguration configuration)
        {
            _adminModel = new AdminModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllAdmins()
        {
            var admins = _adminModel.GetAllAdmins();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public IActionResult GetAdminById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid admin ID.");
            }

            var admin = _adminModel.GetAdminById(id);

            if (admin == null)
            {
                return NotFound("Admin not found.");
            }

            return Ok(admin);
        }

        [HttpPost]
        public IActionResult AddAdmin([FromBody] string username, [FromBody] string password,
            [FromBody] string email, [FromBody] string full_name, [FromBody] int role_id)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Admin username is required.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Admin password is required.");
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Admin email is required.");
            }

            if (string.IsNullOrEmpty(full_name))
            {
                return BadRequest("Admin full name is required.");
            }

            if (role_id == 0)
            {
                return BadRequest("Admin role id is required.");
            }

            if (_adminModel.AddAdmin(username, password, email, full_name, role_id))
            {
                return Ok("Admin added successfully.");
            }
            else
            {
                return BadRequest("Failed to add Admin.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] string username, 
            [FromBody] string password, [FromBody] string email, [FromBody] string full_name,
            [FromBody] int role_id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid admin ID.");
            }

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Admin username is required.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Admin password is required.");
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Admin email is required.");
            }

            if (string.IsNullOrEmpty(full_name))
            {
                return BadRequest("Admin full name is required.");
            }

            if (role_id == 0)
            {
                return BadRequest("Admin role id is required.");
            }

            if (_adminModel.UpdateAdmin(id, username, password, email, full_name, role_id))
            {
                return Ok("Admin updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update admin.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid admin ID.");
            }

            if (_adminModel.DeleteAdmin(id))
            {
                return Ok("Admin deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete admin.");
            }
        }
    }
}

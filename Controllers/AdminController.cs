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

        [HttpGet("id={id}")]
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
        public IActionResult AddAdmin([FromBody] Admin admin)
        {
            if (admin == null)
            {
                return BadRequest("Admin data is required.");
            }

            if (_adminModel.AddAdmin(admin))
            {
                return Ok("Admin added successfully.");
            }
            else
            {
                return BadRequest("Failed to add admin.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] Admin admin)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid admin ID.");
            }

            if (admin == null)
            {
                return BadRequest("Admin data is required.");
            }

            if (_adminModel.UpdateAdmin(id, admin))
            {
                return Ok("Admin updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update admin.");
            }
        }

        [HttpDelete("id={id}")]
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

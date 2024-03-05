using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleModel _roleModel;

        public RoleController(IConfiguration configuration)
        {
            _roleModel = new RoleModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleModel.GetAllRoles();
            return Ok(roles);
        }

        [HttpGet("id={id}")]
        public IActionResult GetRoleById(int id)
        {
            var role = _roleModel.GetRoleById(id);

            if (role == null)
            {
                return NotFound("Role not found.");
            }

            return Ok(role);
        }

        [HttpPost]
        public IActionResult AddRole([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Role name is required.");
            }

            if (_roleModel.AddRole(name))
            {
                return Ok("Role added successfully.");
            }
            else
            {
                return BadRequest("Failed to add role.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateRole(int id, [FromBody] string name, [FromBody] bool active)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Role name is required.");
            }

            if (_roleModel.UpdateRole(id, name, active))
            {
                return Ok("Role updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update role.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeleteRole(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID.");
            }

            if (_roleModel.DeleteRole(id))
            {
                return Ok("Role deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete role.");
            }
        }
    }
}

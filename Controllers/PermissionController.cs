using Microsoft.AspNetCore.Mvc;
using BookStore.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionModel _permissionModel;

        public PermissionController(IConfiguration configuration)
        {
            _permissionModel = new PermissionModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllPermissions()
        {
            var permissions = _permissionModel.GetAllRoles();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public IActionResult GetPermissionById(int id)
        {
            var permission = _permissionModel.GetPermissionById(id);

            if (permission == null)
            {
                return NotFound("Role not found.");
            }

            return Ok(permission);
        }

        [HttpPost]
        public IActionResult AddPermission([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Permission name is required.");
            }

            if (_permissionModel.AddPermission(name))
            {
                return Ok("Permission added successfully.");
            }
            else
            {
                return BadRequest("Failed to add permission.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePermission(int id, [FromBody] string name, [FromBody] bool active)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid permission ID.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Permission name is required.");
            }

            if (_permissionModel.UpdatePermission(id, name, active))
            {
                return Ok("Permission updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update permission.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePermission(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid permission ID.");
            }

            if (_permissionModel.DeletePermission(id))
            {
                return Ok("Permission deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete permission.");
            }
        }
    }
}

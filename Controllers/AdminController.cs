using BookStore.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminModel _adminModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _adminModel = new AdminModel(configuration);
            _httpContextAccessor = httpContextAccessor;
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

        [HttpGet("search")]
        public IActionResult SearchAdmins(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword cannot be empty");
            }

            var admins = _adminModel.SearchAdmins(keyword);
            return Ok(admins);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request.");
            }

            var admin = _adminModel.ValidateAdminCredentials(loginRequest.Username, loginRequest.Password);

            if (admin != null)
            {
                // Store admin information in session
                _httpContextAccessor.HttpContext.Session.SetString("AdminUsername", admin.Username);
                _httpContextAccessor.HttpContext.Session.SetInt32("AdminId", admin.Id);

                return Ok(new { success = true, redirectUrl = "/admin/redirect" });
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
            _httpContextAccessor.HttpContext.Session.Remove("AdminUsername");
            _httpContextAccessor.HttpContext.Session.Remove("AdminId");

            return Ok(new { success = true, redirectUrl = "/admin/login" });
        }

        [HttpGet("redirect")]
        public IActionResult RedirectAdmin()
        {
            if (IsAdminLoggedIn())
            {
                // Admin is logged in, you can redirect to the admin dashboard or perform other actions
                return Ok("Admin is logged in.");
            }
            else
            {
                return Unauthorized("Admin is not logged in.");
            }
        }

        private bool IsAdminLoggedIn()
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32("AdminId").HasValue;
        }

        public class AdminLoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}

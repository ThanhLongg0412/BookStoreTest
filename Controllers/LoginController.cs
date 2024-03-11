using BookStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginModel _loginModel;

        public LoginController(IConfiguration configuration)
        {
            _loginModel = new LoginModel(configuration);
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid username or password.");
            }

            var login = _loginModel.Login(loginRequest);

            if (login == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { login.Username, login.Password });
        }
    }
}

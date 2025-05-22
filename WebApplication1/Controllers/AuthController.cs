using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public class LoginDto
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class AuthResponseDto
        {
            public required string Token { get; set; }
            public required string Username { get; set; }
            public required string Role { get; set; }
        }

        [HttpPost("login")]
        public ActionResult<AuthResponseDto> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto.Username == "admin" && loginDto.Password == "admin")
            {
                var token = _tokenService.GenerateToken(loginDto.Username, "Admin");
                return new AuthResponseDto { Token = token, Username = loginDto.Username, Role = "Admin" };
            }
            if (loginDto.Username == "user" && loginDto.Password == "user")
            {
                var token = _tokenService.GenerateToken(loginDto.Username, "User");
                return new AuthResponseDto { Token = token, Username = loginDto.Username, Role = "User" };
            }
            return Unauthorized("Invalid credentials");
        }
    }
}

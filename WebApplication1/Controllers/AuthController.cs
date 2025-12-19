using Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;



namespace Controllers;

[ApiController]
[Route("api/[controller]")]


public class AuthController : ControllerBase
{

    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;


    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        try
        {

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (result == null)
            {

                return Unauthorized("Invalid email or password");

            }

            return Ok(result);

        }

        catch (Exception ex)
        {

            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "An error occurred during login");

        }

    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {

        try
        {

            var success = await _authService.RegisterAsync(request.Email, request.Password);

            if (!success)
            {

                return BadRequest("Registration failed");

            }

            return Ok(new { message = "User registered successfully" });

        }

        catch (Exception ex)
        {

            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, "An error occurred during registration");

        }

    }


}
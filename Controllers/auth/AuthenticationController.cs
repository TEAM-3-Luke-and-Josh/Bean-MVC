using Microsoft.AspNetCore.Mvc;
using BeanScene.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace BeanScene.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Login attempt received for username: {loginDto.Username}");
                _logger.LogInformation($"Request body: {JsonSerializer.Serialize(loginDto)}");
                
                var response = await _authService.Login(loginDto);
                _logger.LogInformation($"Login response: Success={response.Success}, UserType={response.UserType}");
                
                if (!response.Success)
                {
                    _logger.LogWarning($"Login failed for user {loginDto.Username}");
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Error during login",
                    Token = null!,
                    UserType = null!,
                    Username = null!
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "Error during registration", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public ActionResult GetProfile()
        {
            try
            {
                var username = User.Identity!.Name;
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                return Ok(new
                {
                    Username = username,
                    Role = role,
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile");
                return StatusCode(500, new { message = "Error retrieving profile", error = ex.Message });
            }
        }
    }
}
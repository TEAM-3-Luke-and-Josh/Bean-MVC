using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BeanScene.Models;
using BeanScene.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginDto loginDto);
        Task<AuthResponse> Register(RegisterDto registerDto);
        string GenerateJwtToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly BeanSceneContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(BeanSceneContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponse> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            
            _logger.LogInformation($"Login attempt: User found: {user != null}");
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                _logger.LogWarning("Login failed: Invalid username or password");
                return new AuthResponse { Success = false, Message = "Invalid username or password" };
            }

            var token = GenerateJwtToken(user);
            
            var response = new AuthResponse 
            { 
                Success = true,
                Token = token,
                UserType = user.UserType,
                Username = user.Username,
                Message = "Login successful"
            };
            
            _logger.LogInformation($"Login successful for user: {user.Username}, UserType: {user.UserType}");
            _logger.LogDebug($"Generated token length: {token?.Length ?? 0}");
            
            return response;
        }

        public async Task<AuthResponse> Register(RegisterDto registerDto)
        {
            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return new AuthResponse { Success = false, Message = "Username or email already exists" };
            }

            // Create new user
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User
            {
                Username = registerDto.Username,
                Password = hashedPassword,
                Email = registerDto.Email,
                UserType = "Member" // Default new registrations to Member role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponse 
            { 
                Success = true,
                Token = token,
                UserType = user.UserType,
                Username = user.Username
            };
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType),
                    new Claim("UserId", user.UserID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string UserType { get; set; } = default!;
        public string Username { get; set; } = default!;
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = default!;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
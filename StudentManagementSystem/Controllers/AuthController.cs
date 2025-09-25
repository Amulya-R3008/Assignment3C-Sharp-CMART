using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagementSystem.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Example hardcoded users (you can replace with MongoDB users later)
        private readonly List<(string Username, string Password, string Role)> _users = new()
        {
            ("User1", "password1", "Admin"),
            ("User2", "password2", "Moderator"),
            ("User3", "password3", "ReadOnly")
        };

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            // Find user
            var user = _users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == default)
                return Unauthorized("Invalid username or password.");

            // Ensure secret key is long enough (32+ characters)
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long.");

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60")),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            // Return token
            return Ok(new { Token = jwtToken, Username = user.Username, Role = user.Role });
        }
    }

    // DTO for login request
    public record LoginRequest(string Username, string Password);
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundsInvestorsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken()
        {
            try
            {
                // Create claims (customize as needed)
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role", "Admin")
                };

                // Get secret key
                var secret = _config["Jwt:Key"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    _logger.LogError("JWT secret key is missing from configuration");
                    return StatusCode(500, "JWT configuration is missing");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Create JWT
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("JWT token generated successfully for user {User}", "testuser");

                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                return StatusCode(500, "An error occurred while generating token");
            }
        }
    }
}

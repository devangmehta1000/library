using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IConfiguration config, ILogger<LibraryController> logger) : ControllerBase
    {
        /// <summary>
        /// Generates a token
        /// </summary>
        /// <param name="role">role claim to be added in the token</param>
        /// <returns>token string</returns>
        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation("GenerateToken")]
        public IActionResult GenerateToken(string role = "admin")
        {
            try
            {
                var tokenString = GenerateJwt(role);
                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                ex.Message);
            }
        }

        private string GenerateJwt(string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, role),
                new(ClaimTypes.Name, "123456")
            };

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"], 
                config["Jwt:Audience"],                
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
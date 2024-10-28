using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SupermercadoAPI.API.Models;
using SupermercadoAPI.API.Settings;

namespace SupermercadoAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;

        public AuthController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginModel loginModel)
        {
            if (loginModel.Email != "admin@admin.com" || loginModel.Senha != "admin")
            {
                return Unauthorized();
            }

            var token = GerarJwt();
            return Ok(token);
        }

        private LoginResponse GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin@admin.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Emissor,
                Audience = _jwtSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var response = new LoginResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@admin.com",
                    Claims = claims.Select(c => c.Type)
                }
            };

            return response;
        }
    }
}
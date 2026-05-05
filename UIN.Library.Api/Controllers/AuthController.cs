using Microsoft.AspNetCore.Mvc;
using UIN.Library.Api.Auth;
using UIN.Library.Api.Models;

namespace UIN.Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService = new JwtService();
        private static List<RefreshToken> refreshTokens = new();

        [HttpPost("login")]
        public IActionResult Login(string username, string role)
        {
            var token = _jwtService.GenerateToken(username, role);

            var refreshToken = Guid.NewGuid().ToString();

            refreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Username = username,
                Role = role,
                Expiration = DateTime.UtcNow.AddMinutes(10)
            });

            return Ok(new
            {
                accessToken = token,
                refreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(string refreshToken)
        {
            var storedToken = refreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.Expiration < DateTime.UtcNow)
                return Unauthorized("Refresh token invalide");

            // nouveau JWT
            var newAccessToken = _jwtService.GenerateToken(
                    storedToken.Username,
                    storedToken.Role
                ); // simplifié

            return Ok(new
            {
                accessToken = newAccessToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout(string refreshToken)
        {
            var token = refreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
            }

            return Ok("Déconnecté");
        }
    }
}
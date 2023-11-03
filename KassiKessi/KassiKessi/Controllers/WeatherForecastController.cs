using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace KassiKessi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WeatherForecastController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize] // Lisää Authorize-attribuutti, jotta reitti vaatii JWT-tokenin
        public IActionResult Get()
        {
            // Tässä voit käsitellä pyyntöjä
            // Palautetaan vaikka dummy-tietoa
            var data = new[] { "sunny", "cloudy", "rainy" };
            return Ok(data);
        }

        // api/weatherforecast/token
        // This function generates a JWT-token and returns it to the client as a string in the response body
        [HttpGet("token")]
        public IActionResult GetToken()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            Console.WriteLine($"Issuer: {jwtSettings["Issuer"]}");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }
    };
}
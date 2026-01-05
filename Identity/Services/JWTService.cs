using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Services
{
    public class JWTService
    {
        private readonly IConfiguration _config;

        //public static int TokenExpirationTimeInMinutes = 30;

        public JWTService(IConfiguration config)
        {
            _config = config;

            //Already implemented in Utility.appsettings.json
            //_config["Jwt:ExpireMinutes"] = TokenExpirationTimeInMinutes.ToString();
        }

        public string GenerateToken(int userId, string userName, string privilegeLevel)
        {
            //pobranie ustawień z appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireMinutes = double.Parse(_config["Jwt:ExpireMinutes"]!);

            var claims = new List<Claim>
        {

            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, privilegeLevel)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}



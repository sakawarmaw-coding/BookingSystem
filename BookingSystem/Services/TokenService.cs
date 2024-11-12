using BookingSystem.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly SymmetricSecurityKey _refreshKey;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Token"]));
            _refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:RefreshToken"]));
        }
        public string CreateToken(string username,out DateTime ExpiryDate)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, username)
            };

            //Creating credentials. Specifying which type of Security Algorithm we are using
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            ExpiryDate = DateTime.Now.AddDays(1);
            //Creating Token description
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires=ExpiryDate,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Finally returning the created token
            return tokenHandler.WriteToken(token);
        }

        public string RefreshToken(string username, string deviceId, out DateTime expireDate)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, username),
                new Claim("SessionId", deviceId)
            };

            //Creating credentials. Specifying which type of Security Algorithm we are using
            var creds = new SigningCredentials(_refreshKey, SecurityAlgorithms.HmacSha512Signature);
            expireDate = DateTime.Now.AddDays(1);
            //Creating Token description
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDate,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Finally returning the created token
            return tokenHandler.WriteToken(token);
        }
    }
}

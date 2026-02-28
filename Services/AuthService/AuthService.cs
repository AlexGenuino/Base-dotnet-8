using Domain.Abstractions;
using Domain.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.AuthService
{
    public class AuthService(IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateJWT(string email, string username)
        {
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var key = _configuration["JWT:Key"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new("Email", email),
                new("Username", username),
                new("EmailIdentifier", email.Split('@')[0]),
                new("CurrentTime", DateTime.UtcNow.ToString("O"))
            };

            _ = int.TryParse(_configuration["JWT:TokenExpirationTimeInDays"], out int tokenExpirationTimeInDays);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(tokenExpirationTimeInDays),
                signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[128];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(secureRandomBytes);
            return Convert.ToBase64String(secureRandomBytes);
        }

        public string HashingPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public ValidationFieldsUser? GetValidationErrorForEmailAndUsername(bool emailExists, bool usernameExists)
        {
            if (emailExists && usernameExists)
                return ValidationFieldsUser.UsernameAndEmailUnavailable;
            if (emailExists)
                return ValidationFieldsUser.EmailUnavailable;
            if (usernameExists)
                return ValidationFieldsUser.UsernameUnavailable;
            return null;
        }
    }
}

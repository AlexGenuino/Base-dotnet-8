using Application.UserCQ.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace API.Controllers
{
    public class BaseController(IHttpContextAccessor contextAccessor, IConfiguration configuration) : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
        private readonly IConfiguration _configuration = configuration;

        protected void GenerateAuthCookie(RefreshTokenViewModel response) 
        {
            var cookieOptionsToken = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationTimeInDays"], out int refreshTokenExpirationTimeInDays);

            var cookieOptionsRefreshToken = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpirationTimeInDays)
            };

            Response.Cookies.Append("jwt", response.TokenJWT!, cookieOptionsToken);
            Response.Cookies.Append("refreshToken", response.RefreshToken!, cookieOptionsRefreshToken);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace FriendBook.IdentityServer.API.Domain.JWT
{
    public static class JwtHelper
    {
        public static bool CustomLifeTimeValidator(DateTime? nbf, DateTime? exp, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            return exp != null ? exp > DateTime.UtcNow : false;
        }
        
        public static void SetJwtCookie(this IResponseCookies responseCookies, (string, string, Guid) jwtComponent)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
            };
            responseCookies.Append(CookieNames.JWTToken, jwtComponent.Item1, cookieOptions);
            //responseCookies.Append(CookieNames.RefreshToken, jwtComponent.Item2, cookieOptions);
            responseCookies.Append(CookieNames.AccountId, jwtComponent.Item3.ToString(), cookieOptions);
        }

        public static void RemoveJwtCookie(this IResponseCookies responseCookies)
        {
            responseCookies.Delete(CookieNames.JWTToken);
            //responseCookies.Delete(CookieNames.RefreshToken);
            responseCookies.Delete(CookieNames.AccountId);
        }
    }
}

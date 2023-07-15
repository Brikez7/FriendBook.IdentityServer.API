using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FriendBook.IdentityServer.API.Domain.UserToken;
using FriendBook.IdentityServer.API.Domain.CustomClaims;

namespace FriendBook.IdentityServer.Tests.WebAppFactories
{
    internal static class TokenHelpers
    {
        public static TokenAuth? CreateTokenAuth(string? token, string Issuer, string Audience, string secretKey)
        {
            if(token is null)
                throw new ArgumentNullException(nameof(token));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token))
                return null;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            var login = principal.Claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(principal.Claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new TokenAuth(login, id);
        }
    }
}

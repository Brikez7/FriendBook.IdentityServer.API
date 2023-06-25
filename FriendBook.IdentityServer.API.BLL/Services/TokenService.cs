using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.JWT;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _JWTSettings;

        public TokenService(IOptions<JWTSettings> optionJWTSettings)
        {
            _JWTSettings = optionJWTSettings.Value;
        }

        string ITokenService.GenerateAccessToken(Account account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(CustomClaimType.Login,account.Login),
                new Claim(CustomClaimType.AccountId, account.Id.ToString()!)
            };

            var jwtToken = GenerateToken(_JWTSettings.AccessTokenSecretKey, _JWTSettings.Issuer, _JWTSettings.Audience, _JWTSettings.AccessTokenExpirationMinutes, claims);

            return jwtToken;
        }

        string ITokenService.GenerateRefreshToken(Account account)
        {
            string SecretNumber;
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                SecretNumber = Convert.ToBase64String(randomNumber);
            }

            List<Claim> claims = new List<Claim> 
            {
                new Claim(CustomClaimType.SecretNumber,SecretNumber),
            };

            var jwtToken = GenerateToken(_JWTSettings.RefreshTokenSecretKey, _JWTSettings.Issuer, _JWTSettings.Audience, _JWTSettings.RefreshTokenExpirationMinutes, claims);

            return jwtToken;
            // Добавить в Redis SecretNumber c ключ L`ogin + Id
        }

        private static string GenerateToken(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim>? claims = null)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var jwtToken = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(expires)),
                    notBefore: DateTime.Now,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public BaseResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, string secretKey)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _JWTSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _JWTSettings.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = JwtHelper.CustomLifeTimeValidator
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            if(!tokenHandler.CanReadToken(token))
                return new StandartResponse<ClaimsPrincipal> { Message = "Token not validated", StatusCode = StatusCode.InternalServerError };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return new StandartResponse<ClaimsPrincipal> { Message = "Token not validated", StatusCode = StatusCode.InternalServerError };
            return new StandartResponse<ClaimsPrincipal> { Data = principal };
        }
    }
}

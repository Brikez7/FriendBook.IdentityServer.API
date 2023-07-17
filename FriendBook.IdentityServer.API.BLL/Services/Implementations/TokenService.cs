using FriendBook.IdentityServer.API.BLL.Services.Interfaces;
using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FriendBook.IdentityServer.API.BLL.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _JWTSettings;

        public TokenService(IOptions<JWTSettings> optionJWTSettings)
        {
            _JWTSettings = optionJWTSettings.Value;
        }

        public string GenerateAccessToken(DataAccessToken account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(CustomClaimType.Login,account.Login),
                new Claim(CustomClaimType.AccountId, account.Id.ToString()!)
            };

            var jwtToken = GenerateToken(_JWTSettings.AccessTokenSecretKey, _JWTSettings.Issuer, _JWTSettings.Audience, _JWTSettings.AccessTokenExpirationMinutes, claims);

            return jwtToken;
        }

        public string GenerateRefreshToken(DataAccessToken account, out string SecretNumber)
        {
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
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            if(!tokenHandler.CanReadToken(token))
                return new StandartResponse<ClaimsPrincipal> { Message = "Token not validated", StatusCode = Code.TokenNotValidated };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return new StandartResponse<ClaimsPrincipal> { Message = "Token not validated", StatusCode = Code.TokenNotValidated };
            return new StandartResponse<ClaimsPrincipal> { Data = principal, StatusCode = Code.TokenReadied };
        }
    }
}

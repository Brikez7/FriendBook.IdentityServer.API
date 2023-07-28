using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Services.Interfaces
{
    public interface ITokenService
    {
        public ResponseAuthenticate GenerateAuthenticatedToken(DataAccessToken account, out string SecretNumber)
        {
            var accessToken = GenerateAccessToken(account);
            var refreshToken = GenerateRefreshToken(account, out SecretNumber);

            return new ResponseAuthenticate(accessToken, refreshToken);
        }
        public string GenerateAccessToken(DataAccessToken account);
        public string GenerateRefreshToken(DataAccessToken account, out string SecretNumber);
        public BaseResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, string secretKey);
    }
}

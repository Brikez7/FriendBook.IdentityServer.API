using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface ITokenService
    {
        public ResponseAuthenticated GenerateAuthenticatedToken(DataAccessToken account, out string SecretNumber)
        {
            var accessToken = GenerateAccessToken(account);
            var refreshToken = GenerateRefreshToken(account, out SecretNumber);

            return new ResponseAuthenticated(accessToken, refreshToken);
        }
        public string GenerateAccessToken(DataAccessToken account);
        public string GenerateRefreshToken(DataAccessToken account, out string SecretNumber);
        public BaseResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, string secretKey);
    }
}

using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface ITokenService
    {
        public BaseResponse<ResponseAuthenticated> GenerateAuthenticatedToken(TokenAuth account)
        {
            var accessToken = GenerateAccessToken(account);
            var refreshToken = GenerateRefreshToken(account);

            return new StandartResponse<ResponseAuthenticated>
            {
                Data = new ResponseAuthenticated(accessToken, refreshToken),
                StatusCode = StatusCode.TokenGenerate
            };
        }
        public string GenerateAccessToken(TokenAuth account);
        public string GenerateRefreshToken(TokenAuth account);
        public BaseResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, string secretKey);
    }
}

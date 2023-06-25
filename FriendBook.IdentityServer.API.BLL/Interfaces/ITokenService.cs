using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface ITokenService
    {
        public BaseResponse<AuthenticatedTokenResponse> GenerateAuthenticatedToken(Account account)
        {
            var accessToken = GenerateAccessToken(account);
            var refreshToken = GenerateRefreshToken(account);

            return new StandartResponse<AuthenticatedTokenResponse>
            {
                Data = new AuthenticatedTokenResponse(accessToken, refreshToken)
            };
        }
        protected string GenerateAccessToken(Account account);
        protected string GenerateRefreshToken(Account account);
        public BaseResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, string secretKey);
    }
}

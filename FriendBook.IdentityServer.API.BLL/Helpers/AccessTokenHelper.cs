using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Helpers
{
    public static class AccessTokenHelper
    {
        public static Lazy<AccessToken> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<AccessToken>(() => CreateUserToken(claims));
        }
        public static AccessToken CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new AccessToken(login, id);
        }
        public static BaseResponse<AccessToken?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value;
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            if (stringId == null || login == null)
                return new StandartResponse<AccessToken?> { Message = "Access token not validated", StatusCode = ServiceCode.TokenNotValidated };

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new StandartResponse<AccessToken?> { Data = new AccessToken(login, id) };
        }
    }
}

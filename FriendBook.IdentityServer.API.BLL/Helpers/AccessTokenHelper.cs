using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Helpers
{
    public static class AccessTokenHelper
    {
        public static Lazy<DataAccessToken> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<DataAccessToken>(() => CreateUserToken(claims));
        }
        public static DataAccessToken CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new DataAccessToken(login, id);
        }
        public static BaseResponse<DataAccessToken?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value;
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            if (stringId == null || login == null)
                return new StandartResponse<DataAccessToken?> { Message = "Access token not validated", StatusCode = Code.TokenNotValidated };

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new StandartResponse<DataAccessToken?> { Data = new DataAccessToken(login, id) };
        }
    }
}

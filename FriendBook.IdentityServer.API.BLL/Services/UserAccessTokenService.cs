using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class UserAccessTokenService : IUserAccessTokenService
    {
        public Lazy<UserAccsessToken> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<UserAccsessToken>(() => CreateUserToken(claims));
        }
        private static UserAccsessToken CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new UserAccsessToken(login, id);
        }
        public BaseResponse<UserAccsessToken> CreateUserTokenTryEmpty(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value;
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            if (stringId == null || login == null)
                return new StandartResponse<UserAccsessToken> { Message = "Access token not validated", StatusCode = StatusCode.InternalServerError };

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new StandartResponse<UserAccsessToken> { Data = new UserAccsessToken(login, id) };
        }
    }
}

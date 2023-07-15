using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.IdentityServer.API.BLL.Helpers
{
    public static class AccessTokenHelper
    {
        public static Lazy<TokenAuth> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<TokenAuth>(() => CreateUserToken(claims));
        }
        public static TokenAuth CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new TokenAuth(login, id);
        }
        public static BaseResponse<TokenAuth?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value;
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            if (stringId == null || login == null)
                return new StandartResponse<TokenAuth?> { Message = "Access token not validated", StatusCode = StatusCode.TokenNotValid };

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new StandartResponse<TokenAuth?> { Data = new TokenAuth(login, id) };
        }
    }
}

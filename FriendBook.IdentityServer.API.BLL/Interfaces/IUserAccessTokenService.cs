using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IUserAccessTokenService
    {
        public Lazy<UserAccsessToken> CreateUser(IEnumerable<Claim> claims);
        public BaseResponse<UserAccsessToken> CreateUserTokenTryEmpty(IEnumerable<Claim> claims);
    }
}

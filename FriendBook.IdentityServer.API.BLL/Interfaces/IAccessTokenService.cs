using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IAccessTokenService
    {
        public Lazy<TokenAuth> CreateUser(IEnumerable<Claim> claims);
        public BaseResponse<TokenAuth?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims);
    }
}

using FriendBook.IdentityServer.API.Domain.CustomClaims;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.Domain.UserToken
{
    public class UserTokenAuth
    {
        public string Login { get; set; }
        public Guid Id { get; set; }

        public UserTokenAuth(string login, Guid id)
        {
            Login = login;
            Id = id;
        }
        public static UserTokenAuth CreateUserToken(IEnumerable<Claim> claims) {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new UserTokenAuth(login,id);
        }
    }
}

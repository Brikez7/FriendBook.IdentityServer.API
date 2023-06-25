using FriendBook.IdentityServer.API.Domain.CustomClaims;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.Domain.UserToken
{
    public class UserAccsessToken
    {
        public string Login { get; set; }
        public Guid Id { get; set; }

        public UserAccsessToken(string login, Guid id)
        {
            Login = login;
            Id = id;
        }
        public static UserAccsessToken CreateUserToken(IEnumerable<Claim> claims) {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new UserAccsessToken(login,id);
        }

        public static UserAccsessToken CreateUserTokenЕкнУьзен(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value ?? "";
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new UserAccsessToken(login, id);
        }
    }
}

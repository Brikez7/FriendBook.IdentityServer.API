using FriendBook.IdentityServer.API.Domain.Entities;

namespace FriendBook.IdentityServer.API.Domain.DTO
{
    public class AccountDTO
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

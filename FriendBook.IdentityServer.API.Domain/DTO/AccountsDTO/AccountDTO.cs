using FriendBook.IdentityServer.API.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO
{
    public class AccountDTO
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

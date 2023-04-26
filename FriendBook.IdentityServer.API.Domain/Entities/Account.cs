using FriendBook.IdentityServer.API.Domain.DTO;

namespace FriendBook.IdentityServer.API.Domain.Entities
{
    public class Account
    {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Profession { get; set; }
        public string? Info { get; set; }
        public string? Company { get; set; }
        public string? Telephone { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Account(AccountDTO model, string salt, string password)
        {
            Login = model.Login;
            Password = password;
            CreateDate = DateTime.Now;
            Salt = salt;
        }

        public Account()
        {
        }
    }
}

namespace FriendBook.IdentityServer.API.Domain.UserToken
{
    public class TokenAuth
    {
        public string Login { get; set; }
        public Guid Id { get; set; }

        public TokenAuth(string login, Guid id)
        {
            Login = login;
            Id = id;
        }
    }
}

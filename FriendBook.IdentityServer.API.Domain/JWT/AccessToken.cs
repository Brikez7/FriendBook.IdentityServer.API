namespace FriendBook.IdentityServer.API.Domain.JWT
{
    public class AccessToken
    {
        public string Login { get; set; }
        public Guid Id { get; set; }

        public AccessToken(string login, Guid id)
        {
            Login = login;
            Id = id;
        }
    }
}

namespace FriendBook.IdentityServer.API.Domain.UserToken
{
    public class DataAccessToken
    {
        public string Login { get; set; }
        public Guid Id { get; set; }

        public DataAccessToken(string login, Guid id)
        {
            Login = login;
            Id = id;
        }
    }
}

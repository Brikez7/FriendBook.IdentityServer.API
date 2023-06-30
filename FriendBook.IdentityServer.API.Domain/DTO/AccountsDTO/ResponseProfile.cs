namespace FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO
{
    public class ResponseProfile
    {
        public string Login { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid Id { get; set; }

        public ResponseProfile(Guid id, string login, string? fullName)
        {
            Login = login;
            FullName = fullName;
            Id = id;
        }
    }
}

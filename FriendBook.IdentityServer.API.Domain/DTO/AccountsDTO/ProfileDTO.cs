namespace FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO
{
    public class ProfileDTO
    {
        public ProfileDTO(Guid id, string login, string? fullName)
        {
            Login = login;
            FullName = fullName;
            Id = id;
        }

        public string Login { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid Id { get; set; }
    }
}

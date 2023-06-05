using FriendBook.IdentityServer.API.Domain.Entities;

namespace FriendBook.IdentityServer.API.Domain.DTO.AcouuntsDTO
{
    public class UserContactDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? Info { get; set; }
        public string? Profession { get; set; }
        public string? Telephone { get; set; }

        public UserContactDTO() { }

        public UserContactDTO(string? fullName, string? email, string? login, string? info, string? profession, string? telephone)
        {
            FullName = fullName;
            Email = email;
            Login = login;
            Info = info;
            Profession = profession;
            Telephone = telephone;
        }

        public UserContactDTO(Account entity)
        {
            FullName = entity.FullName;
            Email = entity.Email;
            Login = entity.Login;
            Info = entity.Info;
            Profession = entity.Profession;
            Telephone = entity.Telephone;
        }
    }
}

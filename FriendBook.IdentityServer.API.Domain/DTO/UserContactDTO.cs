namespace FriendBook.IdentityServer.API.Domain.DTO
{
    public class UserContactDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Profession { get; set; }
        public string? Info { get; set; }
        public string? Company { get; set; }
        public string? Telephone { get; set; }

        public UserContactDTO(string? fullName, string? email, string? profession, string? info, string? company, string? telephone)
        {
            FullName = fullName;
            Email = email;
            Profession = profession;
            Info = info;
            Company = company;
            Telephone = telephone;
        }
    }
}

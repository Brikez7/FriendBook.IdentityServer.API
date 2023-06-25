namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IPasswordService
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string Password, byte[] passwordHash, byte[] passwordSalt);
    }
}

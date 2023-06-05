namespace FriendBook.IdentityServer.API.Domain.JWT
{
    public class JWTSettings
    {
        public const double StartJWTTokenLifeTime = 60;
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}

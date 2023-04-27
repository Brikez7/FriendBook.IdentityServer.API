namespace FriendBook.IdentityServer.API.Domain.JWT
{
    public class JWTSettings
    {
        public const double StartJWTTokenLifeTime = 3600;
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
